using MySql.Data.MySqlClient;

namespace Web_Api.Services;

public class ConversationListItem
{
    public int ConversationId { get; set; }
    public int OtherUserId { get; set; }
    public string OtherName { get; set; } = "";
    public string? OtherRole { get; set; }
    public string? LastBody { get; set; }
    public DateTime? LastAt { get; set; }
    public int UnreadCount { get; set; }
}

public class ChatMessageItem
{
    public long Id { get; set; }
    public int SenderId { get; set; }
    public string Body { get; set; } = "";
    public DateTime SentAt { get; set; }
    public bool IsMine { get; set; }
}

// Plain parameterized SQL against Conversations/ChatMessages — same precedent as
// BroadcastStore (follows UserSessions, not the older selector-branch stored proc convention).
public class ChatStore
{
    // DATETIME columns come back from MySql.Data as DateTimeKind.Unspecified — the DB server's
    // system_time_zone is UTC (confirmed), but System.Text.Json only appends 'Z' for Kind=Utc,
    // so an untagged Unspecified value gets serialized with no timezone marker at all and the
    // client's `new Date(iso)` then misreads it as local time instead of UTC. Every DateTime
    // read back from the DB must be re-tagged before it reaches a response DTO.
    private static DateTime AsUtc(object value) => DateTime.SpecifyKind(Convert.ToDateTime(value), DateTimeKind.Utc);

    private readonly string _connect;

    public ChatStore(IConfiguration config)
    {
        _connect = config.GetConnectionString("ConsString")!;
    }

    public async Task<bool> UserExistsAsync(int number)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand("SELECT 1 FROM Users WHERE number = @n", con);
        cmd.Parameters.AddWithValue("@n", number);
        return await cmd.ExecuteScalarAsync() != null;
    }

    private static async Task<int?> SelectConversationIdAsync(MySqlConnection con, int lo, int hi)
    {
        using var sel = new MySqlCommand("SELECT Id FROM Conversations WHERE UserA = @a AND UserB = @b", con);
        sel.Parameters.AddWithValue("@a", lo);
        sel.Parameters.AddWithValue("@b", hi);
        var result = await sel.ExecuteScalarAsync();
        return result is null ? null : Convert.ToInt32(result);
    }

    public async Task<int> GetOrCreateConversationAsync(int userA, int userB)
    {
        var lo = Math.Min(userA, userB);
        var hi = Math.Max(userA, userB);

        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();

        var existing = await SelectConversationIdAsync(con, lo, hi);
        if (existing is not null) return existing.Value;

        try
        {
            using var ins = new MySqlCommand(
                "INSERT INTO Conversations (UserA, UserB) VALUES (@a, @b); SELECT LAST_INSERT_ID();", con);
            ins.Parameters.AddWithValue("@a", lo);
            ins.Parameters.AddWithValue("@b", hi);
            return Convert.ToInt32(await ins.ExecuteScalarAsync());
        }
        catch (MySqlException ex) when (ex.Number == 1062) // duplicate key — lost the race, use the winner's row
        {
            return (await SelectConversationIdAsync(con, lo, hi))!.Value;
        }
    }

    public async Task<bool> IsParticipantAsync(int conversationId, int me)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(
            "SELECT 1 FROM Conversations WHERE Id = @id AND (UserA = @me OR UserB = @me)", con);
        cmd.Parameters.AddWithValue("@id", conversationId);
        cmd.Parameters.AddWithValue("@me", me);
        return await cmd.ExecuteScalarAsync() != null;
    }

    public async Task<int?> GetOtherParticipantAsync(int conversationId, int me)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(
            "SELECT CASE WHEN UserA = @me THEN UserB ELSE UserA END FROM Conversations WHERE Id = @id AND (UserA = @me OR UserB = @me)",
            con);
        cmd.Parameters.AddWithValue("@id", conversationId);
        cmd.Parameters.AddWithValue("@me", me);
        var result = await cmd.ExecuteScalarAsync();
        return result is null ? null : Convert.ToInt32(result);
    }

    public async Task<List<ConversationListItem>> GetConversationsAsync(int me)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(@"
            SELECT c.Id AS ConvId,
                   CASE WHEN c.UserA = @me THEN c.UserB ELSE c.UserA END AS OtherId,
                   u.Name, u.Surname, u.role,
                   (SELECT Body FROM ChatMessages m WHERE m.ConversationId = c.Id ORDER BY m.SentAt DESC LIMIT 1) AS LastBody,
                   (SELECT SentAt FROM ChatMessages m WHERE m.ConversationId = c.Id ORDER BY m.SentAt DESC LIMIT 1) AS LastAt,
                   (SELECT COUNT(*) FROM ChatMessages m WHERE m.ConversationId = c.Id AND m.SenderId <> @me AND m.ReadAt IS NULL) AS UnreadCount
            FROM Conversations c
            JOIN Users u ON u.number = (CASE WHEN c.UserA = @me THEN c.UserB ELSE c.UserA END)
            WHERE c.UserA = @me OR c.UserB = @me
            ORDER BY LastAt IS NULL, LastAt DESC", con);
        cmd.Parameters.AddWithValue("@me", me);

        var results = new List<ConversationListItem>();
        using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
        {
            results.Add(new ConversationListItem
            {
                ConversationId = Convert.ToInt32(dr["ConvId"]),
                OtherUserId = Convert.ToInt32(dr["OtherId"]),
                OtherName = $"{dr["Name"]} {dr["Surname"]}".Trim(),
                OtherRole = dr["role"] is DBNull ? null : dr["role"].ToString(),
                LastBody = dr["LastBody"] is DBNull ? null : dr["LastBody"].ToString(),
                LastAt = dr["LastAt"] is DBNull ? null : AsUtc(dr["LastAt"]),
                UnreadCount = Convert.ToInt32(dr["UnreadCount"])
            });
        }
        return results;
    }

    // Marks the other participant's messages read as a side effect of viewing the thread —
    // standard chat-app behavior, and the ChatMessages.ReadAt column exists specifically for this.
    public async Task<List<ChatMessageItem>> GetThreadAsync(int conversationId, int me)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();

        using (var upd = new MySqlCommand(
            "UPDATE ChatMessages SET ReadAt = NOW() WHERE ConversationId = @id AND SenderId <> @me AND ReadAt IS NULL", con))
        {
            upd.Parameters.AddWithValue("@id", conversationId);
            upd.Parameters.AddWithValue("@me", me);
            await upd.ExecuteNonQueryAsync();
        }

        using var cmd = new MySqlCommand(
            "SELECT Id, SenderId, Body, SentAt FROM ChatMessages WHERE ConversationId = @id ORDER BY SentAt ASC", con);
        cmd.Parameters.AddWithValue("@id", conversationId);

        var results = new List<ChatMessageItem>();
        using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
        {
            var senderId = Convert.ToInt32(dr["SenderId"]);
            results.Add(new ChatMessageItem
            {
                Id = Convert.ToInt64(dr["Id"]),
                SenderId = senderId,
                Body = dr["Body"].ToString() ?? "",
                SentAt = AsUtc(dr["SentAt"]),
                IsMine = senderId == me
            });
        }
        return results;
    }

    public async Task<long> InsertMessageAsync(int conversationId, int senderId, string body)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(@"
            INSERT INTO ChatMessages (ConversationId, SenderId, Body) VALUES (@c, @s, @b);
            SELECT LAST_INSERT_ID();", con);
        cmd.Parameters.AddWithValue("@c", conversationId);
        cmd.Parameters.AddWithValue("@s", senderId);
        cmd.Parameters.AddWithValue("@b", body);
        return Convert.ToInt64(await cmd.ExecuteScalarAsync());
    }
}
