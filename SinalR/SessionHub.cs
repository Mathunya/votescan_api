using Microsoft.AspNetCore.SignalR;

public class SessionHub : Hub
{
    public async Task RegisterSession(string userId)
    {
        // group users by userId
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // optional: cleanup
        await base.OnDisconnectedAsync(exception);
    }
}