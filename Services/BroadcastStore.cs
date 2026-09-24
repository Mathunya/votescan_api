using System.Collections.Concurrent;
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
    public bool HasImage { get; set; }
    public string SenderName { get; set; } = "";
    public string? SenderRole { get; set; }
    public string? SenderDelegation { get; set; }
    public string? SenderProvince { get; set; }
    public string? SenderRegion { get; set; }
    public string? SenderMunicipality { get; set; }
    public string? SenderWard { get; set; }
    // Reach stats for this broadcast, the same for every reader (shown as Sent / Received / Read).
    public int SentCount { get; set; }
    public int ReceivedCount { get; set; }
    public int ReadCount { get; set; }
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

    public async Task<int> InsertBroadcastAsync(
        int senderId, string tier, string? scopeValue, string body, int recipientCount,
        string status = "Approved", byte[]? image = null, string? imageMimeType = null)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(@"
            INSERT INTO Broadcasts (SenderId, Tier, ScopeValue, Body, RecipientCount, Status, Image, ImageMimeType)
            VALUES (@sender, @tier, @scope, @body, @count, @status, @img, @mime);
            SELECT LAST_INSERT_ID();", con);
        cmd.Parameters.AddWithValue("@sender", senderId);
        cmd.Parameters.AddWithValue("@tier", tier);
        cmd.Parameters.AddWithValue("@scope", (object?)scopeValue ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@body", body);
        cmd.Parameters.AddWithValue("@count", recipientCount);
        cmd.Parameters.AddWithValue("@status", status);
        cmd.Parameters.AddWithValue("@img", (object?)image ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@mime", (object?)imageMimeType ?? DBNull.Value);
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    // Broadcast images are visible to the sender or any actual recipient (has a receipt row) —
    // unlike chat's single-conversation scoping, a broadcast can have hundreds of recipients.
    public async Task<(byte[] Bytes, string MimeType)?> GetBroadcastImageAsync(int broadcastId, int requesterId)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(@"
            SELECT b.Image, b.ImageMimeType FROM Broadcasts b
            WHERE b.Id = @id AND b.Image IS NOT NULL
              AND (b.SenderId = @me OR EXISTS (
                  SELECT 1 FROM BroadcastReceipts r WHERE r.BroadcastId = b.Id AND r.RecipientId = @me))", con);
        cmd.Parameters.AddWithValue("@id", broadcastId);
        cmd.Parameters.AddWithValue("@me", requesterId);
        using var dr = await cmd.ExecuteReaderAsync();
        if (!await dr.ReadAsync()) return null;
        return ((byte[])dr["Image"], dr["ImageMimeType"] as string ?? "image/jpeg");
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
        public bool HasImage { get; set; }
    }

    // Looked up by FrappeDocName (not by parsing the webhook's doc name as an int) — keeps the
    // correlation explicit and doesn't assume Frappe's autoname always matches Broadcasts.Id.
    public async Task<PendingBroadcastDetail?> GetByFrappeDocNameAsync(string frappeDocName)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(
            "SELECT Id, SenderId, Tier, ScopeValue, Body, Status, CreatedAt, Image IS NOT NULL AS HasImage FROM Broadcasts WHERE FrappeDocName = @doc", con);
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
            CreatedAt = AsUtc(dr["CreatedAt"]),
            HasImage = Convert.ToBoolean(dr["HasImage"])
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
            SELECT b.Id, b.Body, b.Tier, b.ScopeValue, b.CreatedAt, r.ReadAt, b.Image IS NOT NULL AS HasImage,
                   u.Name AS SenderFirstName, u.Surname AS SenderSurname, u.role AS SenderRole,
                   u.Delegation AS SenderDelegation, u.Province AS SenderProvince, u.Region AS SenderRegion,
                   u.Municipality AS SenderMunicipality, u.Ward AS SenderWard
            FROM BroadcastReceipts r
            JOIN Broadcasts b ON b.Id = r.BroadcastId
            LEFT JOIN Users u ON u.number = b.SenderId
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
                ReadAt = dr["ReadAt"] is DBNull ? null : AsUtc(dr["ReadAt"]),
                HasImage = Convert.ToBoolean(dr["HasImage"]),
                SenderName = $"{dr["SenderFirstName"]} {dr["SenderSurname"]}".Trim(),
                SenderRole = dr["SenderRole"] is DBNull ? null : dr["SenderRole"].ToString(),
                SenderDelegation = dr["SenderDelegation"] is DBNull ? null : dr["SenderDelegation"].ToString(),
                SenderProvince = dr["SenderProvince"] is DBNull ? null : dr["SenderProvince"].ToString(),
                SenderRegion = dr["SenderRegion"] is DBNull ? null : dr["SenderRegion"].ToString(),
                SenderMunicipality = dr["SenderMunicipality"] is DBNull ? null : dr["SenderMunicipality"].ToString(),
                SenderWard = dr["SenderWard"] is DBNull ? null : dr["SenderWard"].ToString()
            });
        }

        // The reader must be closed before this connection runs the follow-up queries below.
        await dr.CloseAsync();

        if (results.Count > 0)
        {
            var ids = results.Select(r => r.Id).ToList();
            await MarkDeliveredAsync(con, userNumber, ids);
            var stats = await GetReachStatsAsync(con, ids);
            foreach (var r in results)
            {
                if (!stats.TryGetValue(r.Id, out var st)) continue;
                r.SentCount = st.Sent;
                r.ReceivedCount = st.Received;
                r.ReadCount = st.Read;
            }
        }
        return results;
    }

    // "Received" = the recipient's app has loaded the broadcast. Nothing recorded this before, so
    // stamp DeliveredAt the first time a receipt is returned in the recipient's inbox. Best-effort:
    // a failed write (e.g. a read-only replica) must never break loading the inbox.
    private static async Task MarkDeliveredAsync(MySqlConnection con, int userNumber, List<int> ids)
    {
        try
        {
            using var cmd = new MySqlCommand($@"
                UPDATE BroadcastReceipts SET DeliveredAt = UTC_TIMESTAMP()
                WHERE RecipientId = @u AND DeliveredAt IS NULL
                  AND BroadcastId IN ({string.Join(",", ids.Select((_, i) => "@b" + i))})", con);
            cmd.Parameters.AddWithValue("@u", userNumber);
            for (int i = 0; i < ids.Count; i++) cmd.Parameters.AddWithValue("@b" + i, ids[i]);
            await cmd.ExecuteNonQueryAsync();
        }
        catch (MySqlException)
        {
            // stats just lag until the next successful load
        }
    }

    // Per-broadcast counts are the same for every reader, and computing them scans every receipt
    // of the broadcast (~12,000 rows for an "everyone" one), so they are cached per node for 60s
    // rather than recomputed on each user's inbox load. "Received" counts DeliveredAt OR ReadAt,
    // because a read receipt is necessarily a received one (older broadcasts have ReadAt only).
    private static readonly TimeSpan StatsTtl = TimeSpan.FromSeconds(60);
    private static readonly ConcurrentDictionary<int, (int Sent, int Received, int Read, DateTime At)> StatsCache = new();
    private static readonly SemaphoreSlim StatsLock = new(1, 1);

    private static async Task<Dictionary<int, (int Sent, int Received, int Read)>> GetReachStatsAsync(
        MySqlConnection con, List<int> ids)
    {
        var now = DateTime.UtcNow;
        var stale = ids.Where(id => !StatsCache.TryGetValue(id, out var c) || now - c.At > StatsTtl).ToList();
        if (stale.Count > 0)
        {
            await StatsLock.WaitAsync();
            try
            {
                using var cmd = new MySqlCommand($@"
                    SELECT BroadcastId, COUNT(*) AS Sent,
                           SUM(DeliveredAt IS NOT NULL OR ReadAt IS NOT NULL) AS Received,
                           SUM(ReadAt IS NOT NULL) AS ReadCount
                    FROM BroadcastReceipts
                    WHERE BroadcastId IN ({string.Join(",", stale.Select((_, i) => "@s" + i))})
                    GROUP BY BroadcastId", con);
                for (int i = 0; i < stale.Count; i++) cmd.Parameters.AddWithValue("@s" + i, stale[i]);
                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    StatsCache[Convert.ToInt32(dr["BroadcastId"])] = (
                        Convert.ToInt32(dr["Sent"]), Convert.ToInt32(dr["Received"]),
                        Convert.ToInt32(dr["ReadCount"]), DateTime.UtcNow);
                }
            }
            catch (MySqlException)
            {
                // keep serving whatever is cached (or zeros) — stats are decoration, not the inbox
            }
            finally { StatsLock.Release(); }
        }

        var result = new Dictionary<int, (int Sent, int Received, int Read)>();
        foreach (var id in ids)
            if (StatsCache.TryGetValue(id, out var c)) result[id] = (c.Sent, c.Received, c.Read);
        return result;
    }

    // Only the recipient themself can mark their own receipt read (RecipientId is matched, not
    // just BroadcastId) — this runs behind [Authorize] with the caller's own resolved number.
    public async Task<int> MarkReadAsync(int broadcastId, int userNumber)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(
            "UPDATE BroadcastReceipts SET ReadAt = UTC_TIMESTAMP() WHERE BroadcastId = @b AND RecipientId = @u AND ReadAt IS NULL",
            con);
        cmd.Parameters.AddWithValue("@b", broadcastId);
        cmd.Parameters.AddWithValue("@u", userNumber);
        return await cmd.ExecuteNonQueryAsync();
    }
}
