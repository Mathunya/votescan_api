using System.Text.Json;
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
    // Client-side compressed before this is sent — server independently re-enforces the 2MB cap.
    public string? ImageBase64 { get; set; }
    public string? ImageMimeType { get; set; }
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
    private readonly FrappeApprovalClient _frappe;
    private readonly IConfiguration _config;
    private readonly ImageQuotaService _imageQuota;

    // Broadcasts resolving to more recipients than this require Frappe review before they go
    // out — see the "moderation/abuse" design discussion. super user is exempt regardless of
    // reach (they're the one doing the reviewing).
    private const int ApprovalRecipientThreshold = 100;

    public MessagingController(
        BroadcastScopeResolver resolver, BroadcastStore store, IHubContext<SessionHub> hubContext,
        FrappeApprovalClient frappe, IConfiguration config, ImageQuotaService imageQuota)
    {
        _resolver = resolver;
        _store = store;
        _hubContext = hubContext;
        _frappe = frappe;
        _config = config;
        _imageQuota = imageQuota;
    }

    [HttpPost]
    [Route("broadcast")]
    public async Task<IActionResult> Broadcast([FromBody] BroadcastRequest request)
    {
        var (image, imageMimeType, imageError) = ImageQuotaService.DecodeAndValidate(request.ImageBase64, request.ImageMimeType);
        if (imageError is not null) return BadRequest(imageError);

        if (string.IsNullOrWhiteSpace(request.Body) && image is null)
            return BadRequest("Body or image is required.");

        var scope = string.IsNullOrEmpty(request.SubTier)
            ? await _resolver.ResolveOwnScopeAsync(User)
            : await _resolver.ResolveSubScopeAsync(User, request.SubTier, request.ScopeValue ?? "");

        if (!scope.Success)
            return BadRequest(scope.Error);

        var senderId = await _resolver.ResolveSenderNumberAsync(User.FindFirst("Cell")?.Value);
        if (senderId is null)
            return Unauthorized("Could not resolve sender.");

        if (image is not null && !await _imageQuota.CanSendImageAsync(senderId.Value))
            return BadRequest($"You've reached today's limit of {ImageQuotaService.MaxImagesPerDay} images. Try again after midnight.");

        var role = User.FindFirst("Role")?.Value ?? "";
        var isSuperUser = string.Equals(role, "super user", StringComparison.OrdinalIgnoreCase);
        var needsApproval = !isSuperUser && scope.RecipientIds.Count > ApprovalRecipientThreshold;

        if (needsApproval)
        {
            var pendingId = await _store.InsertBroadcastAsync(
                senderId.Value, scope.Tier, scope.ScopeValue, request.Body, scope.RecipientIds.Count,
                status: "Pending", image: image, imageMimeType: imageMimeType);
            try
            {
                var senderName = await _resolver.GetSenderDisplayNameAsync(senderId.Value);
                var frappeDocName = await _frappe.SubmitForApprovalAsync(
                    pendingId, senderName, role, User.FindFirst("Cell")?.Value ?? "",
                    scope.Tier, scope.ScopeValue, scope.RecipientIds.Count, request.Body,
                    image, imageMimeType);
                await _store.SetFrappeDocNameAsync(pendingId, frappeDocName);
            }
            catch (Exception)
            {
                // The Broadcasts row stays Pending either way — worst case a reviewer has to be
                // pointed at it manually in Frappe. Not surfacing the failure to the sender as
                // an error since, from their side, the submission itself did succeed.
            }

            return Ok(new
            {
                broadcastId = pendingId,
                status = "pending_approval",
                message = $"Your broadcast reaches {scope.RecipientIds.Count} people, which needs admin approval before it sends. Please alert a Votescan admin to review and approve it."
            });
        }

        var broadcastId = await _store.InsertBroadcastAsync(
            senderId.Value, scope.Tier, scope.ScopeValue, request.Body, scope.RecipientIds.Count,
            image: image, imageMimeType: imageMimeType);
        await _store.InsertReceiptsAsync(broadcastId, scope.RecipientIds);

        // Live push, best-effort — GET /Messaging/mine is the source of truth for anyone
        // whose socket is dropped or the app isn't foregrounded (mobile sockets drop constantly).
        // The image itself isn't pushed over the socket (too heavy for a websocket event) — the
        // client sees hasImage and fetches it separately via GET /broadcast/{id}/image.
        var recipientCells = await _resolver.ResolveCellsAsync(scope.RecipientIds);
        var payload = new
        {
            id = broadcastId,
            body = request.Body,
            tier = scope.Tier,
            scopeValue = scope.ScopeValue,
            createdAt = DateTime.UtcNow,
            hasImage = image is not null
        };
        foreach (var cell in recipientCells)
            await _hubContext.Clients.Group(cell).SendAsync("NewBroadcast", payload);

        return Ok(new { broadcastId, status = "approved", tier = scope.Tier, scopeValue = scope.ScopeValue, recipientCount = scope.RecipientIds.Count });
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

    // Called by a Frappe Webhook (configured on ai.votescan.co.za, not user-facing) when a
    // reviewer approves/rejects an App Broadcast Approval doc, or the doc's own scheduled sweep
    // expires it. [AllowAnonymous] bypasses this controller's [Authorize] — this is
    // system-to-system, authenticated by a shared secret header instead of a user JWT.
    [HttpPost]
    [Route("broadcast/decision")]
    [AllowAnonymous]
    public async Task<IActionResult> BroadcastDecision()
    {
        // Frappe's own Webhook security (enable_security on the Webhook doc), not a hand-rolled
        // header check — it signs the exact JSON body it sends with HMAC-SHA256 over the shared
        // secret and puts the result in X-Frappe-Webhook-Signature. Reading the raw body here
        // (rather than [FromBody]) so the bytes hashed are exactly the bytes that were signed.
        Request.EnableBuffering();
        using var reader = new StreamReader(Request.Body, leaveOpen: true);
        var rawBody = await reader.ReadToEndAsync();
        Request.Body.Position = 0;

        var secret = _config["Frappe:WebhookSecret"];
        var providedSignature = Request.Headers["X-Frappe-Webhook-Signature"].FirstOrDefault();
        if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(providedSignature))
            return Unauthorized();

        var computed = Convert.ToBase64String(
            System.Security.Cryptography.HMACSHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(secret), System.Text.Encoding.UTF8.GetBytes(rawBody)));
        var computedBytes = System.Text.Encoding.UTF8.GetBytes(computed);
        var providedBytes = System.Text.Encoding.UTF8.GetBytes(providedSignature);
        if (computedBytes.Length != providedBytes.Length ||
            !System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(computedBytes, providedBytes))
            return Unauthorized();

        var payload = JsonDocument.Parse(rawBody).RootElement;

        if (!payload.TryGetProperty("name", out var nameEl) || nameEl.GetString() is not { } frappeDocName)
            return BadRequest("Missing 'name'.");
        var status = payload.TryGetProperty("status", out var statusEl) ? statusEl.GetString() : null;
        var rejectionReason = payload.TryGetProperty("rejection_reason", out var reasonEl) ? reasonEl.GetString() : null;

        var pending = await _store.GetByFrappeDocNameAsync(frappeDocName);
        if (pending is null) return NotFound();
        if (pending.Status != "Pending") return Ok(); // already acted on — idempotent no-op on a retry

        // Defense in depth: don't trust Frappe's own 24h guard alone (same principle as never
        // trusting client-supplied scope) — re-check here before actually delivering anything.
        if ((DateTime.UtcNow - pending.CreatedAt).TotalHours > 24)
        {
            await _store.ExpireBroadcastAsync(pending.Id);
            return Ok();
        }

        switch (status)
        {
            case "Approved":
                var recipientIds = await _resolver.ResolveRecipientsForTierAsync(pending.Tier, pending.ScopeValue);
                await _store.InsertReceiptsAsync(pending.Id, recipientIds);
                await _store.ApproveBroadcastAsync(pending.Id, recipientIds.Count);

                var recipientCells = await _resolver.ResolveCellsAsync(recipientIds);
                var pushPayload = new
                {
                    id = pending.Id,
                    body = pending.Body,
                    tier = pending.Tier,
                    scopeValue = pending.ScopeValue,
                    createdAt = pending.CreatedAt,
                    hasImage = pending.HasImage
                };
                foreach (var cell in recipientCells)
                    await _hubContext.Clients.Group(cell).SendAsync("NewBroadcast", pushPayload);
                break;

            case "Rejected":
                await _store.RejectBroadcastAsync(pending.Id, rejectionReason);
                break;

            case "Expired":
                await _store.ExpireBroadcastAsync(pending.Id);
                break;

            default:
                return BadRequest($"Unrecognized status '{status}'.");
        }

        return Ok();
    }

    // Visible to the sender or any actual recipient (checked in BroadcastStore, not here) — not
    // the whole receipt list, so this can't be used to enumerate who else got the broadcast.
    [HttpGet]
    [Route("broadcast/{broadcastId}/image")]
    public async Task<IActionResult> BroadcastImage(int broadcastId)
    {
        var myNumber = await _resolver.ResolveSenderNumberAsync(User.FindFirst("Cell")?.Value);
        if (myNumber is null) return Unauthorized();

        var image = await _store.GetBroadcastImageAsync(broadcastId, myNumber.Value);
        if (image is null) return NotFound();
        return File(image.Value.Bytes, image.Value.MimeType);
    }
}
