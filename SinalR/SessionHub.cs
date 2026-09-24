using Microsoft.AspNetCore.SignalR;
using Web_Api.Services;

public class SessionHub : Hub
{
    private readonly PresenceStore _presenceStore;
    private readonly WelcomeService _welcome;
    private readonly ILogger<SessionHub> _logger;

    public SessionHub(PresenceStore presenceStore, WelcomeService welcome, ILogger<SessionHub> logger)
    {
        _presenceStore = presenceStore;
        _welcome = welcome;
        _logger = logger;
    }

    // Called by the app on start/foreground with its installed versionCode. Records it and, the
    // first time a user reports, triggers the one-off welcome message (see WelcomeService).
    // Best-effort: telemetry must never surface as a hub error on the client.
    public async Task ReportAppVersion(string userId, int appVersion)
    {
        try
        {
            await _welcome.ReportAppVersionAsync(userId, appVersion);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "ReportAppVersion failed for {Cell}", userId);
        }
    }

    public async Task RegisterSession(string userId)
    {
        // group users by userId
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);

        // userId here is Users.Cell (see Login.js / auth_context.js). Called on login, on app
        // foreground, and on the client-side heartbeat while the app is open — that's what
        // keeps LastSeen fresh enough for the "Online" recency window (see PresenceStore).
        await _presenceStore.MarkSeenAsync(userId);
    }

    // Same as ReportAppVersion plus the EAS OTA update id the app is running, so the dashboard can
    // show which release each user is on. Separate method (SignalR hub methods can't be overloaded)
    // so older app builds that call ReportAppVersion keep working unchanged.
    public async Task ReportAppInfo(string userId, int appVersion, string? updateId)
    {
        try
        {
            await _welcome.ReportAppVersionAsync(userId, appVersion, updateId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "ReportAppInfo failed for {Cell}", userId);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // optional: cleanup
        await base.OnDisconnectedAsync(exception);
    }
}
