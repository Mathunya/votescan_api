using System.Security.Claims;
using MySql.Data.MySqlClient;

namespace Web_Api.Services;

public class ScopeResolutionResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string Tier { get; set; } = "";
    public string? ScopeValue { get; set; }
    public List<int> RecipientIds { get; set; } = new();
}

// Resolves who a broadcast reaches, using only the sender's own JWT claims plus
// RoleBroadcastScope/TerritoryHierarchy. A caller-supplied narrower scope (e.g. "just this
// one Ward") is always validated as contained inside the sender's own tier before use —
// scope is never trusted from the client alone.
public class BroadcastScopeResolver
{
    private readonly string _connect;

    public BroadcastScopeResolver(IConfiguration config)
    {
        _connect = config.GetConnectionString("ConsString")!;
    }

    private static string? Claim(ClaimsPrincipal user, string type) => user.FindFirst(type)?.Value;

    // "Xhariep Region" (Users.Region / JWT claim) -> "Xhariep" (TerritoryHierarchy District Code)
    private static string NormalizeRegion(string region) =>
        region.EndsWith(" Region", StringComparison.OrdinalIgnoreCase)
            ? region[..^" Region".Length]
            : region;

    public async Task<(string Tier, string? GeoColumn)?> GetRoleScopeAsync(string role)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(
            "SELECT Tier, GeoColumn FROM RoleBroadcastScope WHERE role = @role", con);
        cmd.Parameters.AddWithValue("@role", role);
        using var dr = await cmd.ExecuteReaderAsync();
        if (!await dr.ReadAsync()) return null;
        var tier = dr.GetString(dr.GetOrdinal("Tier"));
        var geoOrdinal = dr.GetOrdinal("GeoColumn");
        var geoColumn = dr.IsDBNull(geoOrdinal) ? null : dr.GetString(geoOrdinal);
        return (tier, geoColumn);
    }

    // Sender's own full-tier scope: everyone under their own Ward/Region/Zone/etc.
    public async Task<ScopeResolutionResult> ResolveOwnScopeAsync(ClaimsPrincipal user)
    {
        var role = Claim(user, "Role") ?? "";
        var roleScope = await GetRoleScopeAsync(role);
        if (roleScope is null)
            return new ScopeResolutionResult { Success = false, Error = "Role is not permitted to broadcast." };

        var (tier, _) = roleScope.Value;

        return tier switch
        {
            "ALL" => await ResolveAllAsync(),
            "VD" => await ResolveByColumnAsync("Voting_District", Claim(user, "VD"), tier),
            "WARD" => await ResolveByColumnAsync("Ward", Claim(user, "Ward"), tier),
            "REGION" => await ResolveByColumnAsync("Region", Claim(user, "Region"), tier),
            "ZONE" => await ResolveZoneAsync(Claim(user, "Ward")),
            _ => new ScopeResolutionResult { Success = false, Error = $"Unknown tier '{tier}'." }
        };
    }

    // Narrower send: sender picks a specific value within their own broader scope (e.g. LET
    // picking one Ward, BET picking one VD). Validates containment before resolving recipients.
    public async Task<ScopeResolutionResult> ResolveSubScopeAsync(ClaimsPrincipal user, string subTier, string chosenValue)
    {
        var role = Claim(user, "Role") ?? "";
        var roleScope = await GetRoleScopeAsync(role);
        if (roleScope is null)
            return new ScopeResolutionResult { Success = false, Error = "Role is not permitted to broadcast." };

        var (ownTier, _) = roleScope.Value;

        if (!await IsContainedInOwnScopeAsync(user, ownTier, chosenValue))
            return new ScopeResolutionResult { Success = false, Error = "Chosen scope is outside your own scope." };

        return subTier switch
        {
            "VD" => await ResolveByColumnAsync("Voting_District", chosenValue, subTier),
            "WARD" => await ResolveByColumnAsync("Ward", chosenValue, subTier),
            "ZONE" => await ResolveZoneByCodeAsync(chosenValue),
            _ => new ScopeResolutionResult { Success = false, Error = $"Cannot narrow to tier '{subTier}'." }
        };
    }

    // Sender's own scope as a TerritoryHierarchy node code — the root a scope-options browse
    // starts from, and what containment checks walk chosen values back up to.
    public async Task<string?> GetOwnScopeRootCodeAsync(ClaimsPrincipal user, string ownTier) => ownTier switch
    {
        "REGION" => NormalizeRegion(Claim(user, "Region") ?? ""),
        "WARD" => Claim(user, "Ward"),
        "VD" => Claim(user, "VD"),
        "ZONE" => await FindZoneCodeForWardAsync(Claim(user, "Ward")),
        "ALL" => null, // no tree root — ALL isn't a TerritoryHierarchy node
        _ => null
    };

    public async Task<bool> IsWithinOwnScopeAsync(ClaimsPrincipal user, string ownTier, string chosenValue) =>
        await IsContainedInOwnScopeAsync(user, ownTier, chosenValue);

    public async Task<List<(string Code, string UnitType, string? Name)>> GetChildrenAsync(string parentCode)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(
            "SELECT Code, UnitType, Name FROM TerritoryHierarchy WHERE ParentCode = @p ORDER BY UnitType, Name", con);
        cmd.Parameters.AddWithValue("@p", parentCode);
        var results = new List<(string, string, string?)>();
        using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
            results.Add((dr["Code"].ToString() ?? "", dr["UnitType"].ToString() ?? "", dr["Name"]?.ToString()));
        return results;
    }

    private async Task<bool> IsContainedInOwnScopeAsync(ClaimsPrincipal user, string ownTier, string chosenValue)
    {
        if (ownTier == "ALL") return true; // super user, anything goes

        string? ownCode = await GetOwnScopeRootCodeAsync(user, ownTier);
        if (string.IsNullOrEmpty(ownCode)) return false;

        // Walk up TerritoryHierarchy from the chosen node until it reaches ownCode, or runs out
        // of parents. Tree is at most 6 levels deep (Province..VD); 10 is a safety cap only.
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();

        string? current = chosenValue;
        for (int i = 0; i < 10 && current != null; i++)
        {
            if (string.Equals(current, ownCode, StringComparison.OrdinalIgnoreCase)) return true;
            using var cmd = new MySqlCommand("SELECT ParentCode FROM TerritoryHierarchy WHERE Code = @c", con);
            cmd.Parameters.AddWithValue("@c", current);
            current = (await cmd.ExecuteScalarAsync()) as string;
        }
        return false;
    }

    private async Task<string?> FindZoneCodeForWardAsync(string? ward)
    {
        if (string.IsNullOrEmpty(ward)) return null;
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(@"
            SELECT z.Code FROM TerritoryHierarchy w
            JOIN TerritoryHierarchy z ON z.Code = w.ParentCode
            WHERE w.Code = @ward AND w.UnitType = 'Ward' AND z.UnitType = 'Zone'", con);
        cmd.Parameters.AddWithValue("@ward", ward);
        return (await cmd.ExecuteScalarAsync()) as string;
    }

    private async Task<ScopeResolutionResult> ResolveZoneAsync(string? ward)
    {
        var zoneCode = await FindZoneCodeForWardAsync(ward);
        if (zoneCode is null)
            return new ScopeResolutionResult
            {
                Success = false,
                Error = "Your Ward has no Zone in the territory hierarchy — cannot resolve a ZONE-tier broadcast."
            };
        return await ResolveZoneByCodeAsync(zoneCode);
    }

    private async Task<ScopeResolutionResult> ResolveZoneByCodeAsync(string zoneCode)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();

        var wards = new List<string>();
        using (var cmd = new MySqlCommand(
            "SELECT Code FROM TerritoryHierarchy WHERE ParentCode = @zone AND UnitType = 'Ward'", con))
        {
            cmd.Parameters.AddWithValue("@zone", zoneCode);
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync()) wards.Add(dr.GetString(0));
        }

        if (wards.Count == 0)
            return new ScopeResolutionResult { Success = false, Error = $"Zone '{zoneCode}' has no Wards." };

        var ids = await SelectUserNumbersInAsync(con, "Ward", wards);
        return new ScopeResolutionResult { Success = true, Tier = "ZONE", ScopeValue = zoneCode, RecipientIds = ids };
    }

    private async Task<ScopeResolutionResult> ResolveByColumnAsync(string column, string? value, string tier)
    {
        if (string.IsNullOrEmpty(value))
            return new ScopeResolutionResult { Success = false, Error = $"Missing {column} claim for this user." };

        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        // column is never client-controlled — always one of a fixed internal set (Voting_District/Ward/Region).
        using var cmd = new MySqlCommand($"SELECT number FROM Users WHERE {column} = @v", con);
        cmd.Parameters.AddWithValue("@v", value);
        var ids = new List<int>();
        using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync()) ids.Add(dr.GetInt32(0));

        return new ScopeResolutionResult { Success = true, Tier = tier, ScopeValue = value, RecipientIds = ids };
    }

    private async Task<ScopeResolutionResult> ResolveAllAsync()
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand("SELECT number FROM Users", con);
        var ids = new List<int>();
        using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync()) ids.Add(dr.GetInt32(0));
        return new ScopeResolutionResult { Success = true, Tier = "ALL", ScopeValue = null, RecipientIds = ids };
    }

    private static async Task<List<int>> SelectUserNumbersInAsync(MySqlConnection con, string column, List<string> values)
    {
        using var cmd = new MySqlCommand(
            $"SELECT number FROM Users WHERE {column} IN ({string.Join(",", values.Select((_, i) => "@v" + i))})", con);
        for (int i = 0; i < values.Count; i++) cmd.Parameters.AddWithValue("@v" + i, values[i]);
        var ids = new List<int>();
        using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync()) ids.Add(dr.GetInt32(0));
        return ids;
    }

    // The JWT's identity claim is Cell, not Users.number — resolve the sender's own number
    // (needed as Broadcasts.SenderId / a Users FK) from their own validated Cell claim.
    public async Task<int?> ResolveSenderNumberAsync(string? cell)
    {
        if (string.IsNullOrEmpty(cell)) return null;
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand("SELECT number FROM Users WHERE Cell = @cell LIMIT 1", con);
        cmd.Parameters.AddWithValue("@cell", cell);
        var result = await cmd.ExecuteScalarAsync();
        return result is null or DBNull ? null : Convert.ToInt32(result);
    }

    // Maps Users.number -> Users.Cell for SignalR group addressing (SessionHub groups by Cell, not number).
    public async Task<List<string>> ResolveCellsAsync(IEnumerable<int> userNumbers)
    {
        var numbers = userNumbers.Distinct().ToList();
        if (numbers.Count == 0) return new List<string>();

        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();
        using var cmd = new MySqlCommand(
            $"SELECT Cell FROM Users WHERE number IN ({string.Join(",", numbers.Select((_, i) => "@n" + i))}) AND Cell IS NOT NULL AND Cell <> ''",
            con);
        for (int i = 0; i < numbers.Count; i++) cmd.Parameters.AddWithValue("@n" + i, numbers[i]);

        var cells = new List<string>();
        using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync()) cells.Add(dr.GetString(0));
        return cells;
    }
}
