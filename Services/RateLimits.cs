using System.Text.Json;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Web_Api.Services;

// Token bucket limits, per user. Buckets live in each API node's memory, so behind
// HAProxy a user's allowance is roughly the limit times the number of nodes, and a
// deploy refills every bucket. Hard quotas (e.g. slip scans per day) are counted in
// the database instead.
//
// Policies (numbers come from the "RateLimits" config section):
//   slipscan  - POST ResultsSlip/scan, every call is a paid Claude request
//   login     - Login/login and getToken, keyed on the username being tried
//   sensitive - slip photo uploads and broadcasts
//   general   - every other request; only logged until RateLimits:General:Enforce is true
public static class RateLimits
{
    public const string SlipScan = "slipscan";
    public const string Login = "login";
    public const string Sensitive = "sensitive";

    private const string LoginUserKey = "RateLimitLoginUser";

    //never limited: load balancer health checks, the live session hub and API docs
    private static readonly string[] ExemptPaths =
        ["/health", "/WeatherForecast", "/sessionHub", "/swagger"];

    public static IServiceCollection AddVoteScanRateLimits(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = (context, token) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString();
                }
                return new ValueTask(context.HttpContext.Response.WriteAsync(
                    "Too many requests. Please wait a minute and try again.", token));
            };
            options.AddPolicy(SlipScan, context =>
                Bucket(PartitionKey(context), Settings(config, "SlipScan", 10, 1)));
            options.AddPolicy(Login, context =>
                Bucket(LoginKey(context), Settings(config, "Login", 10, 1)));
            options.AddPolicy(Sensitive, context =>
                Bucket(PartitionKey(context), Settings(config, "Sensitive", 20, 1)));
        });
        return services;
    }

    // Runs after authentication so the user is known, before UseRateLimiter.
    public static IApplicationBuilder UseVoteScanRateLimits(
        this IApplicationBuilder app, IConfiguration config)
    {
        var general = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            Bucket(PartitionKey(context), Settings(config, "General", 300, 60)));
        var enforce = config.GetValue<bool>("RateLimits:General:Enforce");

        app.Use(async (context, next) =>
        {
            await RememberLoginUser(context);

            var path = context.Request.Path;
            if (!ExemptPaths.Any(exempt =>
                path.StartsWithSegments(exempt, StringComparison.OrdinalIgnoreCase)))
            {
                using var lease = general.AttemptAcquire(context);
                if (!lease.IsAcquired)
                {
                    var logger = context.RequestServices
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("RateLimits");
                    logger.LogWarning(
                        "General rate limit {Action} for {Key} on {Method} {Path}",
                        enforce ? "blocked" : "would block",
                        PartitionKey(context), context.Request.Method, path);
                    if (enforce)
                    {
                        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        await context.Response.WriteAsync(
                            "Too many requests. Please wait a minute and try again.");
                        return;
                    }
                }
            }
            await next();
        });
        app.UseRateLimiter();
        return app;
    }

    private static RateLimitPartition<string> Bucket(string key, TokenBucketRateLimiterOptions settings) =>
        RateLimitPartition.GetTokenBucketLimiter(key, _ => settings);

    private static TokenBucketRateLimiterOptions Settings(
        IConfiguration config, string name, int tokenLimit, int tokensPerMinute) => new()
    {
        TokenLimit = config.GetValue($"RateLimits:{name}:TokenLimit", tokenLimit),
        TokensPerPeriod = config.GetValue($"RateLimits:{name}:TokensPerPeriod", tokensPerMinute),
        ReplenishmentPeriod = TimeSpan.FromSeconds(
            config.GetValue($"RateLimits:{name}:PeriodSeconds", 60)),
        //reject straight away instead of holding requests in a queue
        QueueLimit = 0,
        AutoReplenishment = true,
    };

    //the logged-in user, or the caller's address before login
    private static string PartitionKey(HttpContext context)
    {
        var cell = context.User.FindFirst("Cell")?.Value;
        return string.IsNullOrEmpty(cell) ? $"ip:{ClientIp(context)}" : $"user:{cell}";
    }

    //login attempts count against the account being tried, not a bucket shared by everyone
    private static string LoginKey(HttpContext context)
    {
        var username = context.Items[LoginUserKey] as string
            ?? context.Request.RouteValues["cell"]?.ToString();
        return string.IsNullOrEmpty(username)
            ? $"ip:{ClientIp(context)}"
            : $"login:{username.Trim().ToLowerInvariant()}";
    }

    //HAProxy passes the phone's address in X-Forwarded-For
    private static string ClientIp(HttpContext context)
    {
        var forwarded = context.Request.Headers["X-Forwarded-For"].ToString();
        if (!string.IsNullOrEmpty(forwarded))
        {
            return forwarded.Split(',')[0].Trim();
        }
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    //reads the username from the login body so the login policy can key on it
    private static async Task RememberLoginUser(HttpContext context)
    {
        if (!HttpMethods.IsPost(context.Request.Method) ||
            !context.Request.Path.Equals("/Login/login", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        context.Request.EnableBuffering();
        try
        {
            using var body = await JsonDocument.ParseAsync(context.Request.Body);
            if (body.RootElement.ValueKind == JsonValueKind.Object &&
                body.RootElement.TryGetProperty("username", out var username) &&
                username.ValueKind == JsonValueKind.String)
            {
                context.Items[LoginUserKey] = username.GetString();
            }
        }
        catch (JsonException)
        {
            //the login action reports a bad body itself
        }
        finally
        {
            context.Request.Body.Position = 0;
        }
    }
}
