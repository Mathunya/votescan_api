using System.Net.Http.Json;

namespace Web_Api.Services;

// OS-level push notifications via Expo's push service (backed by FCM/APNs) — this is what shows
// a banner even when the app isn't foregrounded or running at all, unlike the existing SignalR
// push which only reaches a client with a live socket connection. Best-effort, same as SignalR:
// GET /Messaging/mine and GET /Chat/thread/{id} stay the source of truth either way.
public class PushNotificationService
{
    private readonly HttpClient _http;
    private const int BatchSize = 100; // Expo's own recommended max per request

    public PushNotificationService(HttpClient http)
    {
        _http = http;
    }

    public async Task SendAsync(IEnumerable<string> tokens, string title, string body, object? data = null)
    {
        var tokenList = tokens.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().ToList();
        if (tokenList.Count == 0) return;

        for (int offset = 0; offset < tokenList.Count; offset += BatchSize)
        {
            var batch = tokenList.Skip(offset).Take(BatchSize)
                .Select(t => new { to = t, title, body, data, sound = "default" })
                .ToList();
            try
            {
                await _http.PostAsJsonAsync("https://exp.host/--/api/v2/push/send", batch);
            }
            catch (Exception)
            {
                // best-effort — one bad batch never blocks the rest of a broadcast's delivery
            }
        }
    }
}
