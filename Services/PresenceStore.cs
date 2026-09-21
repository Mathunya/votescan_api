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
    private readonly ILogger<PresenceStore> _logger;

    public PresenceStore(IConfiguration config, ILogger<PresenceStore> logger)
    {
        _connect = config.GetConnectionString("ConsString")!;
        _logger = logger;
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

        // Usage stats — one row per user per SAST day (DAU/WAU, minutes in app). Best-effort:
        // never let this break presence.
        try
        {
            // Heartbeats only ticks up if the previous beat was >=45s ago, so the
            // login/foreground calls landing next to a 60s interval beat don't double count.
            // MariaDB evaluates the ON DUPLICATE KEY assignments left to right, so Heartbeats
            // must be listed before LastSeen or it would see the already-updated LastSeen.
            using var daily = new MySqlCommand(@"
                INSERT INTO UserDailyActivity (Cell, ActivityDate, FirstSeen, LastSeen, Heartbeats)
                VALUES (@cell, DATE(UTC_TIMESTAMP() + INTERVAL 2 HOUR), UTC_TIMESTAMP(), UTC_TIMESTAMP(), 1)
                ON DUPLICATE KEY UPDATE
                    Heartbeats = Heartbeats + (TIMESTAMPDIFF(SECOND, LastSeen, UTC_TIMESTAMP()) >= 45),
                    LastSeen = UTC_TIMESTAMP()", con);
            daily.Parameters.AddWithValue("@cell", cell);
            await daily.ExecuteNonQueryAsync();
        }
        catch (MySqlException ex)
        {
            _logger.LogWarning(ex, "Unable to record daily activity for {Cell}", cell);
        }
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
