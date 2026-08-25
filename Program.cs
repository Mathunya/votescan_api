using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// SignalR's WebSocket transport isn't reliably supported over HTTP/2, so force
// HTTP/1.1 to stop clients (mobile app) from negotiating HTTP/2 via ALPN and
// silently failing the sessionHub WebSocket upgrade.
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });
});

// Add services to the container.
// Configure your default endpoint

builder.Services.AddControllers();
// add SignalR
builder.Services.AddSignalR();
// used to call the WhatsApp Cloud API
builder.Services.AddHttpClient();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
c.SwaggerDoc("v1", new OpenApiInfo
{
Title = "WEB API",
Version = "v1",
Description = "anc votescan opi"
});
c.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
});

//enable cors
builder.Services.AddCors();
builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
}));

var app = builder.Build();
app.UseCors("corspolicy");
// map hub endpoint
app.MapHub<SessionHub>("/sessionHub");

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API");
c.RoutePrefix = string.Empty; // Set the root path for Swagger UI
});
// Configure routing and endpoints
app.UseRouting();
app.UseAuthorization();
#pragma warning disable ASP0014
app.UseEndpoints(e => {});
#pragma warning restore ASP0014

app.UseAuthentication();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();