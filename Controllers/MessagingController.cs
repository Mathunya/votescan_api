using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Web_Api.Services;

namespace Web_Api.Controllers;

public class BroadcastRequest
{
    // null SubTier/ScopeValue = send to the sender's own full tier scope.
    // Set both to narrow (e.g. a LET Coordinator picking one Ward) — validated server-side
    // against the sender's own scope, never trusted as-is.
    public string? SubTier { get; set; }
    public string? ScopeValue { get; set; }
    public string Body { get; set; } = "";
}

// Broadcast/chat endpoints. [Authorize] is scoped to this controller only —
// the other ~80 existing controllers are untouched per the build plan.
[Route("[controller]")]
[ApiController]
[Authorize]
public class MessagingController : ControllerBase
{
    private readonly BroadcastScopeResolver _resolver;
    private readonly BroadcastStore _store;
    private readonly IHubContext<SessionHub> _hubContext;

    public MessagingController(BroadcastScopeResolver resolver, BroadcastStore store, IHubContext<SessionHub> hubContext)
    {
        _resolver = resolver;
        _store = store;
        _hubContext = hubContext;
    }

    [HttpPost]
    [Route("broadcast")]
    public async Task<IActionResult> Broadcast([FromBody] BroadcastRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Body))
            return BadRequest("Body is required.");

        var scope = string.IsNullOrEmpty(request.SubTier)
            ? await _resolver.ResolveOwnScopeAsync(User)
            : await _resolver.ResolveSubScopeAsync(User, request.SubTier, request.ScopeValue ?? "");

        if (!scope.Success)
            return BadRequest(scope.Error);

        var senderId = await _resolver.ResolveSenderNumberAsync(User.FindFirst("Cell")?.Value);
        if (senderId is null)
            return Unauthorized("Could not resolve sender.");

        var broadcastId = await _store.InsertBroadcastAsync(
            senderId.Value, scope.Tier, scope.ScopeValue, request.Body, scope.RecipientIds.Count);
        await _store.InsertReceiptsAsync(broadcastId, scope.RecipientIds);

        // Live push, best-effort — GET /Messaging/mine is the source of truth for anyone
        // whose socket is dropped or the app isn't foregrounded (mobile sockets drop constantly).
        var recipientCells = await _resolver.ResolveCellsAsync(scope.RecipientIds);
        var payload = new
        {
            id = broadcastId,
            body = request.Body,
            tier = scope.Tier,
            scopeValue = scope.ScopeValue,
            createdAt = DateTime.UtcNow
        };
        foreach (var cell in recipientCells)
            await _hubContext.Clients.Group(cell).SendAsync("NewBroadcast", payload);

        return Ok(new { broadcastId, tier = scope.Tier, scopeValue = scope.ScopeValue, recipientCount = scope.RecipientIds.Count });
    }

    // Lets the composer drill down through the sender's own scope (e.g. LET -> pick a Zone or
    // Ward within their Region) without the client ever inventing a target value itself. With no
    // `parent`, returns the immediate children of the sender's own scope root. Any `parent`
    // passed back must be validated as inside that same scope before its children are listed.
    [HttpGet]
    [Route("scope-options")]
    public async Task<IActionResult> ScopeOptions(string? parent)
    {
        var role = User.FindFirst("Role")?.Value ?? "";
        var roleScope = await _resolver.GetRoleScopeAsync(role);
        if (roleScope is null) return BadRequest("Role is not permitted to broadcast.");
        var (tier, _) = roleScope.Value;

        if (tier == "ALL")
            return Ok(new { parent = (string?)null, tier, children = Array.Empty<object>() });

        var rootCode = await _resolver.GetOwnScopeRootCodeAsync(User, tier);
        if (rootCode is null)
            return BadRequest("Could not resolve your own scope.");

        var effectiveParent = string.IsNullOrEmpty(parent) ? rootCode : parent;

        if (!string.Equals(effectiveParent, rootCode, StringComparison.OrdinalIgnoreCase) &&
            !await _resolver.IsWithinOwnScopeAsync(User, tier, effectiveParent))
        {
            return BadRequest("Outside your own scope.");
        }

        var children = await _resolver.GetChildrenAsync(effectiveParent);
        return Ok(new
        {
            parent = effectiveParent,
            tier,
            children = children.Select(c => new { code = c.Code, unitType = c.UnitType, name = c.Name })
        });
    }

    [HttpGet]
    [Route("mine")]
    public async Task<IActionResult> Mine()
    {
        var myNumber = await _resolver.ResolveSenderNumberAsync(User.FindFirst("Cell")?.Value);
        if (myNumber is null) return Unauthorized();

        var items = await _store.GetInboxForUserAsync(myNumber.Value);
        return Ok(items);
    }

    [HttpPost]
    [Route("mine/{broadcastId}/read")]
    public async Task<IActionResult> MarkRead(int broadcastId)
    {
        var myNumber = await _resolver.ResolveSenderNumberAsync(User.FindFirst("Cell")?.Value);
        if (myNumber is null) return Unauthorized();

        var rows = await _store.MarkReadAsync(broadcastId, myNumber.Value);
        return rows > 0 ? Ok() : NotFound();
    }
}
