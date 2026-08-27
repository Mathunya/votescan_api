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

    public async Task<int> InsertBroadcastAsync(int senderId, string tier, string? scopeValue, string body, int recipientCount, string status = "Approved")
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(@"
            INSERT INTO Broadcasts (SenderId, Tier, ScopeValue, Body, RecipientCount, Status)
            VALUES (@sender, @tier, @scope, @body, @count, @status);
            SELECT LAST_INSERT_ID();", con);
        cmd.Parameters.AddWithValue("@sender", senderId);
        cmd.Parameters.AddWithValue("@tier", tier);
        cmd.Parameters.AddWithValue("@scope", (object?)scopeValue ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@body", body);
        cmd.Parameters.AddWithValue("@count", recipientCount);
        cmd.Parameters.AddWithValue("@status", status);
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task SetFrappeDocNameAsync(int broadcastId, string frappeDocName)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand("UPDATE Broadcasts SET FrappeDocName = @doc WHERE Id = @id", con);
        cmd.Parameters.AddWithValue("@doc", frappeDocName);
        cmd.Parameters.AddWithValue("@id", broadcastId);
        await cmd.ExecuteNonQueryAsync();
    }

    public class PendingBroadcastDetail
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string Tier { get; set; } = "";
        public string? ScopeValue { get; set; }
        public string Body { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

    // Looked up by FrappeDocName (not by parsing the webhook's doc name as an int) — keeps the
    // correlation explicit and doesn't assume Frappe's autoname always matches Broadcasts.Id.
    public async Task<PendingBroadcastDetail?> GetByFrappeDocNameAsync(string frappeDocName)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(
            "SELECT Id, SenderId, Tier, ScopeValue, Body, Status, CreatedAt FROM Broadcasts WHERE FrappeDocName = @doc", con);
        cmd.Parameters.AddWithValue("@doc", frappeDocName);
        using var dr = await cmd.ExecuteReaderAsync();
        if (!await dr.ReadAsync()) return null;
        return new PendingBroadcastDetail
        {
            Id = Convert.ToInt32(dr["Id"]),
            SenderId = Convert.ToInt32(dr["SenderId"]),
            Tier = dr["Tier"].ToString() ?? "",
            ScopeValue = dr["ScopeValue"] is DBNull ? null : dr["ScopeValue"].ToString(),
            Body = dr["Body"].ToString() ?? "",
            Status = dr["Status"].ToString() ?? "",
            CreatedAt = AsUtc(dr["CreatedAt"])
        };
    }

    // Approval decision: delivery (BroadcastReceipts insert + RecipientCount) is done by the
    // caller via InsertReceiptsAsync, same as the immediate-send path — this just flips status.
    public async Task ApproveBroadcastAsync(int broadcastId, int recipientCount)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(
            "UPDATE Broadcasts SET Status = 'Approved', RecipientCount = @count, ReviewedAt = UTC_TIMESTAMP() WHERE Id = @id", con);
        cmd.Parameters.AddWithValue("@count", recipientCount);
        cmd.Parameters.AddWithValue("@id", broadcastId);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task RejectBroadcastAsync(int broadcastId, string? reason)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(
            "UPDATE Broadcasts SET Status = 'Rejected', RejectionReason = @reason, ReviewedAt = UTC_TIMESTAMP() WHERE Id = @id", con);
        cmd.Parameters.AddWithValue("@reason", (object?)reason ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@id", broadcastId);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task ExpireBroadcastAsync(int broadcastId)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(
            "UPDATE Broadcasts SET Status = 'Expired', ReviewedAt = UTC_TIMESTAMP() WHERE Id = @id AND Status = 'Pending'", con);
        cmd.Parameters.AddWithValue("@id", broadcastId);
        await cmd.ExecuteNonQueryAsync();
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
