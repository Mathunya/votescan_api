using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Web_Api.Services;

namespace Web_Api.Controllers;

// In-app forms (v1: the pre-election coordinator audit). The forms themselves and all the
// workflow rules live in Frappe (votescan_ai.api.coordinator_audit); this controller only
// authenticates the app user, decides whether Forms is switched on for them, and forwards the
// call with their cell number taken from the JWT, never from the request.
//
// Switch (AppFlags):
//   forms_mode       = off | test | live   (default off)
//   forms_test_cells = comma-separated cells always allowed (test or live)
//   live also allows anyone in the FormsAudience table (maintained by Frappe).
[Route("[controller]")]
[ApiController]
[Authorize]
public class FormsController : ControllerBase
{
    private static readonly HashSet<string> Actions = new(StringComparer.Ordinal)
    {
        "tasks", "review_list", "submit_review", "finish_review", "vd_users", "nominate", "respond_nomination",
        "submit_self_check", "request_change"
    };

    private const string FrappeMethod = "votescan_ai.votescan_ai.api.coordinator_audit.app_call";
    private static readonly TimeSpan FlagTtl = TimeSpan.FromSeconds(60);
    private static (string Mode, HashSet<string> Cells, DateTime At) _flags = ("off", new(), DateTime.MinValue);

    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpFactory;
    private readonly WelcomeService _welcome;
    private readonly ILogger<FormsController> _logger;

    public FormsController(IConfiguration config, IHttpClientFactory httpFactory, WelcomeService welcome,
        ILogger<FormsController> logger)
    {
        _config = config;
        _httpFactory = httpFactory;
        _welcome = welcome;
        _logger = logger;
    }

    private string? Cell => User.FindFirst("Cell")?.Value;

    private async Task<(string Mode, HashSet<string> Cells)> FlagsAsync()
    {
        if (DateTime.UtcNow - _flags.At < FlagTtl) return (_flags.Mode, _flags.Cells);
        var mode = "off";
        var cells = new HashSet<string>();
        using var con = new MySqlConnection(_config.GetConnectionString("ConsString"));
        await con.OpenAsync();
        using var cmd = new MySqlCommand(
            "SELECT FlagName, FlagValue FROM AppFlags WHERE FlagName IN ('forms_mode','forms_test_cells')", con);
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            var name = r.GetString(0);
            var value = r.IsDBNull(1) ? "" : r.GetString(1);
            if (name == "forms_mode") mode = value.Trim().ToLowerInvariant();
            else foreach (var c in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                cells.Add(c);
        }
        _flags = (mode, cells, DateTime.UtcNow);
        return (mode, cells);
    }

    // live = only people Frappe has put in FormsAudience (audited coordinators, confirmers, pending
    // nominees), so the other ~13k users never see an empty Forms card or trigger Frappe work.
    private async Task<bool> EnabledForAsync(string? cell)
    {
        if (string.IsNullOrEmpty(cell)) return false;
        var (mode, cells) = await FlagsAsync();
        if (cells.Contains(cell)) return mode is "test" or "live";
        if (mode != "live") return false;
        using var con = new MySqlConnection(_config.GetConnectionString("ConsString"));
        await con.OpenAsync();
        using var cmd = new MySqlCommand("SELECT 1 FROM FormsAudience WHERE Cell = @c LIMIT 1", con);
        cmd.Parameters.AddWithValue("@c", cell);
        return await cmd.ExecuteScalarAsync() is not null;
    }

    // The app calls this on start/foreground to decide whether to show the Forms tab.
    [HttpGet]
    [Route("enabled")]
    public async Task<IActionResult> Enabled() => Ok(new { enabled = await EnabledForAsync(Cell) });

    [HttpPost]
    [Route("{name}")]
    public async Task<IActionResult> Call(string name, [FromBody] JsonElement? payload)
    {
        var action = name;
        if (!Actions.Contains(action)) return NotFound();
        var cell = Cell;
        if (!await EnabledForAsync(cell)) return StatusCode(403, new { message = "Forms are not available yet." });

        var section = _config.GetSection("Frappe");
        var baseUrl = (section["BaseUrl"] ?? "").TrimEnd('/');
        using var req = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/api/method/{FrappeMethod}");
        req.Headers.Authorization = new AuthenticationHeaderValue("token", $"{section["ApiKey"]}:{section["ApiSecret"]}");
        var body = JsonSerializer.Serialize(new
        {
            action,
            cell,
            payload = payload.HasValue && payload.Value.ValueKind == JsonValueKind.Object ? payload.Value.GetRawText() : "{}"
        });
        req.Content = new StringContent(body, Encoding.UTF8, "application/json");

        var http = _httpFactory.CreateClient();
        http.Timeout = TimeSpan.FromSeconds(30);
        HttpResponseMessage resp;
        try { resp = await http.SendAsync(req); }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Forms: Frappe unreachable for {Action}", action);
            return StatusCode(502, new { message = "The forms service is not reachable. Please try again." });
        }

        var text = await resp.Content.ReadAsStringAsync();
        JsonDocument? doc = null;
        try { doc = JsonDocument.Parse(text); } catch (JsonException) { }
        using var _ = doc;
        if (resp.IsSuccessStatusCode && doc is not null && doc.RootElement.TryGetProperty("message", out var msg))
            return Content(msg.GetRawText(), "application/json");

        // Frappe validation errors (frappe.throw) come back as 417 with the text in _server_messages.
        return StatusCode(resp.StatusCode == System.Net.HttpStatusCode.ExpectationFailed ? 400 : 502,
            new { message = FrappeErrorText(doc) ?? "Something went wrong. Please try again." });
    }

    private static string? FrappeErrorText(JsonDocument? doc)
    {
        try
        {
            if (doc is null || !doc.RootElement.TryGetProperty("_server_messages", out var sm)) return null;
            var list = JsonSerializer.Deserialize<string[]>(sm.GetString() ?? "[]");
            if (list is null || list.Length == 0) return null;
            using var first = JsonDocument.Parse(list[0]);
            var m = first.RootElement.GetProperty("message").GetString();
            return m is null ? null : System.Text.RegularExpressions.Regex.Replace(m, "<.*?>", "");
        }
        catch { return null; }
    }

    // Frappe -> API: deliver in-app messages from the Votescan Team account (confirmation
    // messages, nominations). Signed like the broadcast decision webhook: base64 HMAC-SHA256 of
    // the raw body with Frappe:WebhookSecret, in X-Frappe-Webhook-Signature.
    [HttpPost]
    [Route("notify")]
    [AllowAnonymous]
    public async Task<IActionResult> Notify()
    {
        Request.EnableBuffering();
        using var reader = new StreamReader(Request.Body, leaveOpen: true);
        var rawBody = await reader.ReadToEndAsync();
        Request.Body.Position = 0;

        var secret = _config["Frappe:WebhookSecret"];
        var provided = Request.Headers["X-Frappe-Webhook-Signature"].FirstOrDefault();
        if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(provided)) return Unauthorized();
        var computed = Convert.ToBase64String(System.Security.Cryptography.HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(rawBody)));
        var a = Encoding.UTF8.GetBytes(computed);
        var b = Encoding.UTF8.GetBytes(provided);
        if (a.Length != b.Length || !System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(a, b))
            return Unauthorized();

        using var doc = JsonDocument.Parse(rawBody);
        if (!doc.RootElement.TryGetProperty("messages", out var messages) || messages.ValueKind != JsonValueKind.Array)
            return BadRequest();

        int sent = 0, skipped = 0;
        foreach (var m in messages.EnumerateArray())
        {
            var cell = m.TryGetProperty("cell", out var c) ? c.GetString() : null;
            var text = m.TryGetProperty("text", out var t) ? t.GetString() : null;
            if (string.IsNullOrWhiteSpace(cell) || string.IsNullOrWhiteSpace(text) || text.Length > 2000) { skipped++; continue; }
            // Optional image (the weekly team report). Same validation as chat images; the system
            // account isn't subject to the per-user daily image quota.
            byte[]? image = null;
            string? mime = null;
            if (m.TryGetProperty("imageBase64", out var ib) && ib.GetString() is { Length: > 0 } b64)
            {
                var (bytes, imageMime, error) = ImageQuotaService.DecodeAndValidate(
                    b64, m.TryGetProperty("imageMimeType", out var im) ? im.GetString() : "image/jpeg");
                if (error is not null) { skipped++; continue; }
                image = bytes;
                mime = imageMime;
            }
            // Optional PDF (the weekly team report). PDFs only, up to 10 MB, checked by magic bytes.
            string? fileName = null;
            if (image is null && m.TryGetProperty("fileBase64", out var fb) && fb.GetString() is { Length: > 0 } f64)
            {
                byte[] pdf;
                try { pdf = Convert.FromBase64String(f64); } catch { skipped++; continue; }
                var isPdf = pdf.Length > 4 && pdf[0] == 0x25 && pdf[1] == 0x50 && pdf[2] == 0x44 && pdf[3] == 0x46; // %PDF
                if (!isPdf || pdf.Length > 10 * 1024 * 1024) { skipped++; continue; }
                image = pdf;
                mime = "application/pdf";
                fileName = m.TryGetProperty("fileName", out var fnEl) ? fnEl.GetString() : null;
                fileName = string.IsNullOrWhiteSpace(fileName) ? "VoteScan-report.pdf"
                    : new string(fileName.Where(ch => char.IsLetterOrDigit(ch) || ch is '-' or '_' or '.' or ' ').ToArray()).Trim();
                if (fileName.Length > 120) fileName = fileName[..120];
                if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)) fileName += ".pdf";
            }
            if (await _welcome.SendSystemMessageAsync(cell, text, image, mime, fileName)) sent++; else skipped++;
        }
        return Ok(new { sent, skipped });
    }
}
