using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Web_Api.Services;

// Talks to the App Broadcast Approval doctype on ai.votescan.co.za (the existing votescan_ai
// Frappe app). Frappe's own Webhook feature (configured on that side, not here) calls back into
// MessagingController's decision endpoint when a reviewer approves/rejects/it expires — this
// client only handles the outbound "submit for review" direction.
public class FrappeApprovalClient
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;
    private readonly string _apiKey;
    private readonly string _apiSecret;

    public FrappeApprovalClient(IConfiguration config, HttpClient http)
    {
        _http = http;
        var section = config.GetSection("Frappe");
        _baseUrl = (section["BaseUrl"] ?? "").TrimEnd('/');
        _apiKey = section["ApiKey"] ?? "";
        _apiSecret = section["ApiSecret"] ?? "";
    }

    private void AddAuth(HttpRequestMessage req) =>
        req.Headers.Authorization = new AuthenticationHeaderValue("token", $"{_apiKey}:{_apiSecret}");

    // Uploads the image first (if any) via Frappe's generic file-upload endpoint, then creates
    // the doctype record referencing the returned file_url — Frappe's "Attach Image" field
    // stores a URL string, not the raw bytes, so this has to be two calls, in this order.
    public async Task<string> SubmitForApprovalAsync(
        int broadcastId, string senderName, string senderRole, string senderCell,
        string tier, string? scopeValue, int recipientCountEstimate, string messageText,
        byte[]? image, string? imageMimeType)
    {
        string? fileUrl = null;
        if (image is { Length: > 0 })
        {
            fileUrl = await UploadFileAsync(image, imageMimeType, $"broadcast-{broadcastId}");
        }

        using var req = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/resource/App Broadcast Approval");
        AddAuth(req);
        req.Content = JsonContent.Create(new
        {
            votescan_broadcast_id = broadcastId.ToString(),
            sender_name = senderName,
            sender_role = senderRole,
            sender_number = senderCell,
            tier,
            scope_value = scopeValue,
            recipient_count_estimate = recipientCountEstimate,
            message_text = messageText,
            image = fileUrl
        });

        using var resp = await _http.SendAsync(req);
        resp.EnsureSuccessStatusCode();
        var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("data").GetProperty("name").GetString()
               ?? throw new InvalidOperationException("Frappe did not return a document name.");
    }

    private async Task<string> UploadFileAsync(byte[] bytes, string? mimeType, string fileNameHint)
    {
        var ext = mimeType switch
        {
            "image/png" => "png",
            "image/webp" => "webp",
            _ => "jpg"
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/method/upload_file");
        AddAuth(req);
        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType ?? "image/jpeg");
        content.Add(fileContent, "file", $"{fileNameHint}.{ext}");
        content.Add(new StringContent("0"), "is_private");
        req.Content = content;

        using var resp = await _http.SendAsync(req);
        resp.EnsureSuccessStatusCode();
        var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("message").GetProperty("file_url").GetString()
               ?? throw new InvalidOperationException("Frappe did not return a file_url.");
    }
}
