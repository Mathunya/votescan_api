using System.Collections.Concurrent;

namespace Web_Api.Services;

// Limits FAILED logins only. Failures are counted per username and caller address, so
// someone guessing passwords from another phone cannot lock the real user out, and a
// correct password always clears the count. Counts live in each node's memory and
// drop by one per period (RateLimits:Login:MaxFailures / PeriodSeconds).
//
// Many app users reach the API through the Datafree proxy and share its address, so a
// guesser on that same address would still share the real user's count; that is the
// one case where a user could be held back, and only for as long as the guessing lasts.
public class LoginThrottle
{
    private readonly int _maxFailures;
    private readonly TimeSpan _period;
    private readonly ConcurrentDictionary<string, Entry> _failures = new();

    private sealed class Entry
    {
        public double Count;
        public DateTime Updated;
    }

    public LoginThrottle(IConfiguration config)
    {
        _maxFailures = config.GetValue("RateLimits:Login:MaxFailures", 10);
        _period = TimeSpan.FromSeconds(config.GetValue("RateLimits:Login:PeriodSeconds", 60));
    }

    public static string Key(string? username, HttpContext context) =>
        $"{(username ?? "").Trim().ToLowerInvariant()}|{RateLimits.ClientIp(context)}";

    public bool IsBlocked(string key) =>
        _failures.TryGetValue(key, out var entry) && Current(entry) >= _maxFailures;

    public void RecordFailure(string key)
    {
        var entry = _failures.GetOrAdd(key, _ => new Entry { Updated = DateTime.UtcNow });
        lock (entry)
        {
            entry.Count = Current(entry) + 1;
            entry.Updated = DateTime.UtcNow;
        }
        //drop entries that have fully decayed so the table cannot grow without limit
        if (_failures.Count > 50_000)
        {
            foreach (var pair in _failures)
            {
                if (Current(pair.Value) <= 0)
                {
                    _failures.TryRemove(pair.Key, out _);
                }
            }
        }
    }

    public void Reset(string key) => _failures.TryRemove(key, out _);

    //failures fade by one each period
    private double Current(Entry entry) =>
        Math.Max(0, entry.Count - (DateTime.UtcNow - entry.Updated) / _period);
}
