using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Web_Api.Services;

namespace Web_Api.Controllers;

public class SendMessageRequest
{
    public string Body { get; set; } = "";
}

// Phase 2 — one-to-one chat. [Authorize] here too, same reasoning as MessagingController:
// every participant identity comes from the caller's own validated JWT (via Cell -> number),
// never from a client-supplied id, so nobody can read or post into someone else's conversation.
[Route("[controller]")]
[ApiController]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly ChatStore _store;
    private readonly BroadcastScopeResolver _resolver;
    private readonly IHubContext<SessionHub> _hubContext;

    public ChatController(ChatStore store, BroadcastScopeResolver resolver, IHubContext<SessionHub> hubContext)
    {
        _store = store;
        _resolver = resolver;
        _hubContext = hubContext;
    }

    private Task<int?> MeAsync() => _resolver.ResolveSenderNumberAsync(User.FindFirst("Cell")?.Value);

    [HttpPost]
    [Route("conversations/with/{otherUserId}")]
    public async Task<IActionResult> StartConversation(int otherUserId)
    {
        var me = await MeAsync();
        if (me is null) return Unauthorized();
        if (me == otherUserId) return BadRequest("Cannot start a conversation with yourself.");
        if (!await _store.UserExistsAsync(otherUserId)) return NotFound("User not found.");

        var conversationId = await _store.GetOrCreateConversationAsync(me.Value, otherUserId);
        return Ok(new { conversationId });
    }

    [HttpGet]
    [Route("conversations")]
    public async Task<IActionResult> Conversations()
    {
        var me = await MeAsync();
        if (me is null) return Unauthorized();
        return Ok(await _store.GetConversationsAsync(me.Value));
    }

    [HttpGet]
    [Route("thread/{conversationId}")]
    public async Task<IActionResult> Thread(int conversationId)
    {
        var me = await MeAsync();
        if (me is null) return Unauthorized();
        if (!await _store.IsParticipantAsync(conversationId, me.Value)) return Forbid();

        return Ok(await _store.GetThreadAsync(conversationId, me.Value));
    }

    [HttpPost]
    [Route("thread/{conversationId}/messages")]
    public async Task<IActionResult> SendMessage(int conversationId, [FromBody] SendMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Body))
            return BadRequest("Body is required.");

        var me = await MeAsync();
        if (me is null) return Unauthorized();
        if (!await _store.IsParticipantAsync(conversationId, me.Value)) return Forbid();

        var body = request.Body.Trim();
        var messageId = await _store.InsertMessageAsync(conversationId, me.Value, body);
        var sentAt = DateTime.UtcNow;

        var otherId = await _store.GetOtherParticipantAsync(conversationId, me.Value);
        if (otherId is not null)
        {
            var cells = await _resolver.ResolveCellsAsync(new[] { otherId.Value });
            var payload = new { id = messageId, conversationId, senderId = me.Value, body, sentAt };
            foreach (var cell in cells)
                await _hubContext.Clients.Group(cell).SendAsync("NewChatMessage", payload);
        }

        return Ok(new { id = messageId, conversationId, sentAt });
    }
}
