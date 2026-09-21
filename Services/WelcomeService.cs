using Microsoft.AspNetCore.SignalR;
using MySql.Data.MySqlClient;

namespace Web_Api.Services;

// Records the app version a client reports and, the first time a user reports one, drops a
// one-off welcome message into their inbox from the dedicated "Votescan Team" account (a Users
// row with the non-dialable Cell 'VOTESCAN'). Reporting a version is what proves the installed
// app can actually show chat, so it doubles as the trigger.
//
// Guarantees:
//  - Once per user: Users.WelcomeSentAt is claimed with a conditional UPDATE, so only the request
//    that flips it NULL -> timestamp sends anything, even with concurrent logins/foregrounds.
//  - Off by default: nothing is sent unless AppFlags 'welcome_enabled' = '1' (cached ~60s per node).
//  - Best-effort: callers wrap this; a failure after the claim releases it so the next report retries.
// Also answers anyone who messages the Votescan Team account with a canned "not monitored" reply,
// at most once an hour per conversation.
public class WelcomeService
{
    public const string SystemCell = "VOTESCAN";

    public const string WelcomeText =
        "Welcome to Votescan Chat! 👋\n\n" +
        "You can now message anyone on Votescan directly in the app: your coordinator, your team, a fellow volunteer. Share photos too.\n\n" +
        "How to start:\n" +
        "1. Tap Messages\n" +
        "2. Search by 10-digit cell number\n" +
        "3. Tap their name and say hi\n\n" +
        "You'll see when they're online, and a red number shows unread messages.\n" +
        "Important announcements from leadership appear under Broadcasts.\n\n" +
        "This is an automatic message from an unmonitored number, so replies here aren't read. Chat with your team instead.";

    public const string AutoReplyText =
        "This is an automated Votescan number and isn't monitored, so we can't read replies here. " +
        "To chat, tap Messages, search a colleague's cell number and message them. " +
        "For app help, please contact your coordinator.";

    private static readonly TimeSpan FlagTtl = TimeSpan.FromSeconds(60);
    private static readonly SemaphoreSlim SystemLock = new(1, 1);
    private static int? _systemNumber;
    private static bool _flagValue;
    private static DateTime _flagCheckedAt = DateTime.MinValue;

    private readonly string _connect;
    private readonly ChatStore _store;
    private readonly BroadcastScopeResolver _resolver;
    private readonly IHubContext<SessionHub> _hub;
    private readonly ILogger<WelcomeService> _logger;

    public WelcomeService(
        IConfiguration config, ChatStore store, BroadcastScopeResolver resolver,
        IHubContext<SessionHub> hub, ILogger<WelcomeService> logger)
    {
        _connect = config.GetConnectionString("ConsString")!;
        _store = store;
        _resolver = resolver;
        _hub = hub;
        _logger = logger;
    }

    // users.number of the Votescan Team account, looked up once per process. Null if the account
    // doesn't exist (then nothing here does anything).
    public async Task<int?> GetSystemNumberAsync()
    {
        if (_systemNumber.HasValue) return _systemNumber;
        await SystemLock.WaitAsync();
        try
        {
            if (_systemNumber.HasValue) return _systemNumber;
            _systemNumber = await _resolver.ResolveSenderNumberAsync(SystemCell);
            return _systemNumber;
        }
        finally { SystemLock.Release(); }
    }

    private async Task<bool> WelcomeEnabledAsync()
    {
        if (DateTime.UtcNow - _flagCheckedAt < FlagTtl) return _flagValue;
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand("SELECT FlagValue FROM AppFlags WHERE FlagName = 'welcome_enabled'", con);
        var v = await cmd.ExecuteScalarAsync();
        _flagValue = v is not null and not DBNull && v.ToString() == "1";
        _flagCheckedAt = DateTime.UtcNow;
        return _flagValue;
    }

    public async Task ReportAppVersionAsync(string cell, int appVersion)
    {
        if (string.IsNullOrWhiteSpace(cell) || cell == SystemCell) return;

        // Only write when something changed or the last report is over an hour old — the app
        // reports on every foreground, no need to rewrite the row each time.
        using (var con = new MySqlConnection(_connect))
        {
            await con.OpenAsync();
            using var cmd = new MySqlCommand(@"
                UPDATE Users
                SET AppVersion = @v, AppVersionReportedAt = UTC_TIMESTAMP()
                WHERE Cell = @cell
                  AND (AppVersion IS NULL OR AppVersion <> @v
                       OR AppVersionReportedAt IS NULL
                       OR AppVersionReportedAt < UTC_TIMESTAMP() - INTERVAL 1 HOUR)", con);
            cmd.Parameters.AddWithValue("@v", appVersion);
            cmd.Parameters.AddWithValue("@cell", cell);
            await cmd.ExecuteNonQueryAsync();
        }

        if (await WelcomeEnabledAsync())
            await TrySendWelcomeAsync(cell);
    }

    private async Task TrySendWelcomeAsync(string cell)
    {
        var system = await GetSystemNumberAsync();
        if (system is null) return;
        var me = await _resolver.ResolveSenderNumberAsync(cell);
        if (me is null || me.Value == system.Value) return;

        // Claim: only the request that flips WelcomeSentAt NULL -> now goes on to send.
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        int claimed;
        using (var claim = new MySqlCommand(
            "UPDATE Users SET WelcomeSentAt = UTC_TIMESTAMP() WHERE Cell = @cell AND WelcomeSentAt IS NULL", con))
        {
            claim.Parameters.AddWithValue("@cell", cell);
            claimed = await claim.ExecuteNonQueryAsync();
        }
        if (claimed == 0) return;

        try
        {
            var conversationId = await _store.GetOrCreateConversationAsync(system.Value, me.Value);
            var messageId = await _store.InsertMessageAsync(conversationId, system.Value, WelcomeText);
            await NotifyAsync(cell, messageId, conversationId, system.Value, WelcomeText);
        }
        catch
        {
            // Release the claim so the next report retries instead of silently skipping this user.
            using var release = new MySqlCommand("UPDATE Users SET WelcomeSentAt = NULL WHERE Cell = @cell", con);
            release.Parameters.AddWithValue("@cell", cell);
            await release.ExecuteNonQueryAsync();
            throw;
        }
    }

    // Called after a user sends a message into a conversation with the Votescan Team account.
    public async Task MaybeAutoReplyAsync(int conversationId, int userNumber)
    {
        var system = await GetSystemNumberAsync();
        if (system is null) return;

        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using (var recent = new MySqlCommand(@"
            SELECT COUNT(*) FROM ChatMessages
            WHERE ConversationId = @c AND SenderId = @s AND Body = @b
              AND SentAt > UTC_TIMESTAMP() - INTERVAL 1 HOUR", con))
        {
            recent.Parameters.AddWithValue("@c", conversationId);
            recent.Parameters.AddWithValue("@s", system.Value);
            recent.Parameters.AddWithValue("@b", AutoReplyText);
            if (Convert.ToInt32(await recent.ExecuteScalarAsync()) > 0) return;
        }

        var messageId = await _store.InsertMessageAsync(conversationId, system.Value, AutoReplyText);
        var cells = await _resolver.ResolveCellsAsync(new[] { userNumber });
        foreach (var c in cells)
            await NotifyAsync(c, messageId, conversationId, system.Value, AutoReplyText);
    }

    // Same live event the chat controller raises, so an open app shows the message immediately.
    private Task NotifyAsync(string cell, long messageId, int conversationId, int senderId, string body)
    {
        var payload = new { id = messageId, conversationId, senderId, body, sentAt = DateTime.UtcNow, hasImage = false };
        return _hub.Clients.Group(cell).SendAsync("NewChatMessage", payload);
    }
}
