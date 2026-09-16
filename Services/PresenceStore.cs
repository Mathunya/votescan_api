using MySql.Data.MySqlClient;

namespace Web_Api.Services;

// Last-seen presence, backed by a new UserLastSeen table (one row per Cell). Deliberately not
// a live "socket is open" tracker — that needs correct SignalR disconnect handling across two
// api nodes (api-1/api-2), with real crash-safety caveats. Last-seen needs none of that: one
// write path (RegisterSession, called on login/foreground and on a client-side heartbeat while
// the app is open), one read path, naturally correct across nodes since it's just a DB row.
// "Online" is derived from recency (see OnlineWindow) rather than stored as its own flag.
public class PresenceStore
{
    public static readonly TimeSpan OnlineWindow = TimeSpan.FromMinutes(2);

    private readonly string _connect;

    public PresenceStore(IConfiguration config)
    {
        _connect = config.GetConnectionString("ConsString")!;
    }

    public async Task MarkSeenAsync(string cell)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(@"
            INSERT INTO UserLastSeen (Cell, LastSeen)
            VALUES (@cell, UTC_TIMESTAMP())
            ON DUPLICATE KEY UPDATE LastSeen = UTC_TIMESTAMP()", con);
        cmd.Parameters.AddWithValue("@cell", cell);
        await cmd.ExecuteNonQueryAsync();
    }

    public class PresenceInfo
    {
        public DateTime LastSeen { get; set; }
        public bool Online { get; set; }
    }

    // Users.number -> last-seen info, for a batch of numbers (a conversations list, a VD Team
    // list). Numbers with no matching Cell or no UserLastSeen row are simply absent from the
    // result — callers treat "absent" as "never seen"/offline.
    public async Task<Dictionary<int, PresenceInfo>> GetPresenceMapAsync(List<int> numbers)
    {
        var result = new Dictionary<int, PresenceInfo>();
        if (numbers.Count == 0) return result;

        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand($@"
            SELECT u.number, s.LastSeen
            FROM Users u
            JOIN UserLastSeen s ON s.Cell = u.Cell
            WHERE u.number IN ({string.Join(",", numbers.Select((_, i) => "@n" + i))})", con);
        for (int i = 0; i < numbers.Count; i++) cmd.Parameters.AddWithValue("@n" + i, numbers[i]);

        var now = DateTime.UtcNow;
        using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
        {
            var lastSeen = DateTime.SpecifyKind(Convert.ToDateTime(dr["LastSeen"]), DateTimeKind.Utc);
            result[Convert.ToInt32(dr["number"])] = new PresenceInfo
            {
                LastSeen = lastSeen,
                Online = now - lastSeen <= OnlineWindow
            };
        }
        return result;
    }
}
