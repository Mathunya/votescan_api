using Microsoft.AspNetCore.SignalR;
using Web_Api.Services;

public class SessionHub : Hub
{
    private readonly PresenceStore _presenceStore;

    public SessionHub(PresenceStore presenceStore)
    {
        _presenceStore = presenceStore;
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

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // optional: cleanup
        await base.OnDisconnectedAsync(exception);
    }
}
