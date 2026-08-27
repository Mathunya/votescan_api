using MySql.Data.MySqlClient;

namespace Web_Api.Services;

// Shared image rules for both chat and broadcast: 2MB cap (post client-side compression — this
// is the server's hard backstop, never trust the client's own compression), 5 images/day/user
// pooled across chat + broadcast combined, reset at SAST midnight (not UTC — this app's users
// are all in South Africa, and a UTC-midnight reset would silently land at 2am local).
public class ImageQuotaService
{
    private readonly string _connect;
    public const int MaxImagesPerDay = 5;
    public const int MaxImageBytes = 2 * 1024 * 1024;

    private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp"
    };

    public ImageQuotaService(IConfiguration config)
    {
        _connect = config.GetConnectionString("ConsString")!;
    }

    private static DateTime CurrentSastDayStartUtc()
    {
        var sastNow = DateTime.UtcNow.AddHours(2); // SAST = UTC+2, no DST
        return DateTime.SpecifyKind(sastNow.Date.AddHours(-2), DateTimeKind.Utc);
    }

    public async Task<int> CountImagesSentTodayAsync(int senderId)
    {
        var dayStart = CurrentSastDayStartUtc();
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();

        int total = 0;
        using (var cmd = new MySqlCommand(
            "SELECT COUNT(*) FROM ChatMessages WHERE SenderId=@s AND Image IS NOT NULL AND SentAt >= @start", con))
        {
            cmd.Parameters.AddWithValue("@s", senderId);
            cmd.Parameters.AddWithValue("@start", dayStart);
            total += Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }
        using (var cmd = new MySqlCommand(
            "SELECT COUNT(*) FROM Broadcasts WHERE SenderId=@s AND Image IS NOT NULL AND CreatedAt >= @start", con))
        {
            cmd.Parameters.AddWithValue("@s", senderId);
            cmd.Parameters.AddWithValue("@start", dayStart);
            total += Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }
        return total;
    }

    public async Task<bool> CanSendImageAsync(int senderId) => await CountImagesSentTodayAsync(senderId) < MaxImagesPerDay;

    // Decodes + validates a client-supplied base64 image. A null/empty base64 is not an error —
    // it just means this particular message/broadcast has no image, which is the common case.
    public static (byte[]? Bytes, string? MimeType, string? Error) DecodeAndValidate(string? base64, string? mimeType)
    {
        if (string.IsNullOrEmpty(base64)) return (null, null, null);

        if (string.IsNullOrEmpty(mimeType) || !AllowedMimeTypes.Contains(mimeType))
            return (null, null, "Unsupported image type — use JPEG, PNG, or WebP.");

        byte[] bytes;
        try { bytes = Convert.FromBase64String(base64); }
        catch (FormatException) { return (null, null, "Invalid image data."); }

        if (bytes.Length > MaxImageBytes)
            return (null, null, $"Image is too large — max {MaxImageBytes / (1024 * 1024)}MB after compression.");

        return (bytes, mimeType, null);
    }
}
