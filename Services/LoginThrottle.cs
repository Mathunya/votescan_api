using System.Collections.Concurrent;

namespace Web_Api.Services;

// Limits FAILED logins only. Failures are counted per username and caller address, and
// a correct password always clears the count. Counts live in each node's memory and
// drop by one per period (RateLimits:Login:MaxFailures / PeriodSeconds).
//
// The caller address does not tell phones apart in production: the DC2 router rewrites
// every source address to 192.168.0.1, so the key is effectively the username alone and
// anyone who knows a cell number could lock that user out. RateLimits:Login:Enforce is
// therefore false by default: failures are still counted, and a login that would be
// blocked is logged (at most once a minute per key) and allowed. When true, blocked
// logins get 429. A per-install device ID from the app could make the key safe later.
public class LoginThrottle
{
    private readonly int _maxFailures;
    private readonly TimeSpan _period;
    private readonly ConcurrentDictionary<string, Entry> _failures = new();
    private readonly ConcurrentDictionary<string, DateTime> _lastWarned = new();
    private readonly ILogger<LoginThrottle> _logger;

    public bool Enforce { get; }

    private sealed class Entry
    {
        public double Count;
        public DateTime Updated;
    }

    public LoginThrottle(IConfiguration config, ILogger<LoginThrottle> logger)
    {
        _logger = logger;
        _maxFailures = config.GetValue("RateLimits:Login:MaxFailures", 10);
        _period = TimeSpan.FromSeconds(config.GetValue("RateLimits:Login:PeriodSeconds", 60));
        Enforce = config.GetValue("RateLimits:Login:Enforce", false);
    }

    public static string Key(string? username, HttpContext context) =>
        $"{(username ?? "").Trim().ToLowerInvariant()}|{RateLimits.ClientIp(context)}";

    // True when this login must be refused. Without enforcement a login that would have
    // been refused is only logged, with the cell number masked to its last 4 digits.
    public bool ShouldBlock(string key, string? username)
    {
        if (!_failures.TryGetValue(key, out var entry) || Current(entry) < _maxFailures)
        {
            return false;
        }
        if (Enforce)
        {
            return true;
        }
        var now = DateTime.UtcNow;
        var last = _lastWarned.GetValueOrDefault(key);
        if (now - last >= TimeSpan.FromMinutes(1))
        {
            _lastWarned[key] = now;
            _logger.LogWarning(
                "Login limit would block {Cell} after {Failures} failed attempts (not enforced)",
                Mask(username), (int)Current(entry));
        }
        return false;
    }

    public void RecordFailure(string key)
    {
        var entry = _failures.GetOrAdd(key, _ => new Entry { Updated = DateTime.UtcNow });
        lock (entry)
        {
            entry.Count = Current(entry) + 1;
            entry.Updated = DateTime.UtcNow;
        }
        //drop entries that have fully decayed so the tables cannot grow without limit
        if (_failures.Count > 50_000)
        {
            foreach (var pair in _failures)
            {
                if (Current(pair.Value) <= 0)
                {
                    _failures.TryRemove(pair.Key, out _);
                    _lastWarned.TryRemove(pair.Key, out _);
                }
            }
        }
    }

    public void Reset(string key)
    {
        _failures.TryRemove(key, out _);
        _lastWarned.TryRemove(key, out _);
    }

    //failures fade by one each period
    private double Current(Entry entry) =>
        Math.Max(0, entry.Count - (DateTime.UtcNow - entry.Updated) / _period);

    //0821234567 -> ******4567
    private static string Mask(string? cell)
    {
        var value = (cell ?? "").Trim();
        return value.Length <= 4 ? "****" : new string('*', value.Length - 4) + value[^4..];
    }
}
