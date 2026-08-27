using MySql.Data.MySqlClient;

namespace Web_Api.Services;

public class BroadcastListItem
{
    public int Id { get; set; }
    public string Body { get; set; } = "";
    public string Tier { get; set; } = "";
    public string? ScopeValue { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; } // null = unread; set = read, kept visible for 24h from this timestamp
}

// Plain parameterized SQL against Broadcasts/BroadcastReceipts — follows the precedent set by
// UserSessions (LoginController/SignalRController) rather than the older selector-branch stored
// proc convention, since this is a new table with no existing proc to extend.
public class BroadcastStore
{
    private readonly string _connect;

    public BroadcastStore(IConfiguration config)
    {
        _connect = config.GetConnectionString("ConsString")!;
    }

    public async Task<int> InsertBroadcastAsync(int senderId, string tier, string? scopeValue, string body, int recipientCount)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(@"
            INSERT INTO Broadcasts (SenderId, Tier, ScopeValue, Body, RecipientCount)
            VALUES (@sender, @tier, @scope, @body, @count);
            SELECT LAST_INSERT_ID();", con);
        cmd.Parameters.AddWithValue("@sender", senderId);
        cmd.Parameters.AddWithValue("@tier", tier);
        cmd.Parameters.AddWithValue("@scope", (object?)scopeValue ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@body", body);
        cmd.Parameters.AddWithValue("@count", recipientCount);
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task InsertReceiptsAsync(int broadcastId, List<int> recipientIds)
    {
        if (recipientIds.Count == 0) return;

        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();

        const int batchSize = 200;
        for (int offset = 0; offset < recipientIds.Count; offset += batchSize)
        {
            var batch = recipientIds.Skip(offset).Take(batchSize).ToList();
            var values = string.Join(",", batch.Select((_, i) => $"(@b, @r{i})"));
            using var cmd = new MySqlCommand(
                $"INSERT INTO BroadcastReceipts (BroadcastId, RecipientId) VALUES {values}", con);
            cmd.Parameters.AddWithValue("@b", broadcastId);
            for (int i = 0; i < batch.Count; i++) cmd.Parameters.AddWithValue($"@r{i}", batch[i]);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    // DATETIME columns come back from MySql.Data as DateTimeKind.Unspecified — the DB server's
    // system_time_zone is UTC (confirmed), but System.Text.Json only appends 'Z' for Kind=Utc,
    // so an untagged Unspecified value serializes with no timezone marker and the client's
    // `new Date(iso)` then misreads it as local time instead of UTC.
    private static DateTime AsUtc(object value) => DateTime.SpecifyKind(Convert.ToDateTime(value), DateTimeKind.Utc);

    // Inbox — unread items, plus items read within the last 24h (kept visible so a tap doesn't
    // make a notice vanish instantly; it just stops counting as unread and ages out a day later).
    public async Task<List<BroadcastListItem>> GetInboxForUserAsync(int userNumber)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(@"
            SELECT b.Id, b.Body, b.Tier, b.ScopeValue, b.CreatedAt, r.ReadAt
            FROM BroadcastReceipts r
            JOIN Broadcasts b ON b.Id = r.BroadcastId
            WHERE r.RecipientId = @user
              AND (r.ReadAt IS NULL OR r.ReadAt > UTC_TIMESTAMP() - INTERVAL 24 HOUR)
            ORDER BY b.CreatedAt DESC", con);
        cmd.Parameters.AddWithValue("@user", userNumber);

        var results = new List<BroadcastListItem>();
        using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
        {
            results.Add(new BroadcastListItem
            {
                Id = Convert.ToInt32(dr["Id"]),
                Body = dr["Body"].ToString() ?? "",
                Tier = dr["Tier"].ToString() ?? "",
                ScopeValue = dr["ScopeValue"] is DBNull ? null : dr["ScopeValue"].ToString(),
                CreatedAt = AsUtc(dr["CreatedAt"]),
                ReadAt = dr["ReadAt"] is DBNull ? null : AsUtc(dr["ReadAt"])
            });
        }
        return results;
    }

    // Only the recipient themself can mark their own receipt read (RecipientId is matched, not
    // just BroadcastId) — this runs behind [Authorize] with the caller's own resolved number.
    public async Task<int> MarkReadAsync(int broadcastId, int userNumber)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(
            "UPDATE BroadcastReceipts SET ReadAt = NOW() WHERE BroadcastId = @b AND RecipientId = @u AND ReadAt IS NULL",
            con);
        cmd.Parameters.AddWithValue("@b", broadcastId);
        cmd.Parameters.AddWithValue("@u", userNumber);
        return await cmd.ExecuteNonQueryAsync();
    }
}
