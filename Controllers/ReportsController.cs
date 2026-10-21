using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using Web_Api.Models;

namespace Web_Api.Controllers;

[Route("[controller]")]
[ApiController]
public class ReportsController : ControllerBase
{
    private IConfiguration? _config;
    private Report? r;
    private string? connect;

    public ReportsController(IConfiguration configuration)
    {
        _config = configuration;
        connect = _config.GetConnectionString("ConsString");
        r = new Report();
    }

    [HttpGet]
    [Route("getprovincialnumbers")]
    public IEnumerable<Report> getprovincialnumbers()
    {
        MySqlDataReader dr;
        List<Report> all = new List<Report>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getReports", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@sender", "provincial");
                cmd.AddMissingStoredProcedureParameters();

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    r = new Report();
                    r.Municipality = dr["municipality"].ToString();
                    r.Special = dr["Special"].ToString();
                    r.Recorded = dr["Recorded"].ToString();
                    r.Total_1 = dr["Total_1"].ToString();

                    all.Add(r);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    [HttpGet]
    [Route("getattitude")]
    public IEnumerable<Report> getattitude()
    {
        MySqlDataReader dr;
        List<Report> all = new List<Report>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getReports", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@sender", "attitude");
                cmd.AddMissingStoredProcedureParameters();

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    r = new Report();
                    r.Municipality = dr["municipality"].ToString();
                    r.Total = dr["voters"].ToString();
                    r.Anc_supporters = dr["supporters"].ToString();
                    r.Undecided = dr["undecided"].ToString();
                    r.Non_supporters = dr["nonsupporters"].ToString();
                    r.Not_captured = dr["notcaptured"].ToString();

                    all.Add(r);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    [HttpGet]
    [Route("getattitudebylet/{municipality}")]
    public IEnumerable<Report> getattitudebylet(string municipality)
    {
        MySqlDataReader dr;
        List<Report> all = new List<Report>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getReports", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@sender", "attitudebylet");
                cmd.Parameters.AddWithValue("@municipality", municipality);
                cmd.AddMissingStoredProcedureParameters();

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    r = new Report();
                    r.Municipality = dr["municipality"].ToString();
                    r.Total = dr["voters"].ToString();
                    r.Anc_supporters = dr["supporters"].ToString();
                    r.Undecided = dr["undecided"].ToString();
                    r.Non_supporters = dr["nonsupporters"].ToString();
                    r.Not_captured = dr["notcaptured"].ToString();

                    all.Add(r);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    [HttpGet]
    [Route("getattitudebyregion/{region}")]
    public IEnumerable<Report> getattitudebyregion(string region)
    {
        MySqlDataReader dr;
        List<Report> all = new List<Report>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getReports", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@region", region);
                cmd.Parameters.AddWithValue("@sender", "byregion");
                cmd.AddMissingStoredProcedureParameters();

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    r = new Report();
                    r.Municipality = dr["municipality"].ToString();
                    r.Total = dr["voters"].ToString();
                    r.Anc_supporters = dr["supporters"].ToString();
                    r.Undecided = dr["undecided"].ToString();
                    r.Non_supporters = dr["nonsupporters"].ToString();
                    r.Not_captured = dr["notcaptured"].ToString();

                    all.Add(r);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    // Powers the Regional Attitude pie chart's "Issues" mode on the dashboard
    // Home page: one row per (municipality-or-zone, issue) pair within the
    // given region, using the same recursive-CTE issue-splitting logic as
    // getissues() above, scoped via getReports' "issuesbyscope" sender.
    // Mangaung has no municipalities of its own, so that region's rows are
    // grouped by zone instead — mirrors getattitudebyscope's region=Mangaung
    // branch exactly.
    [HttpGet]
    [Route("getissuesbyscope/{region}")]
    public IEnumerable<IssueScope> getissuesbyscope(string region, [FromQuery] string? timeframe)
    {
        MySqlDataReader dr;
        List<IssueScope> all = new List<IssueScope>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getReports", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@region", region);
                cmd.Parameters.AddWithValue("@sender", "issuesbyscope");
                // "description" is an otherwise-unused parameter on getReports —
                // repurposed here (and on getattitudebyscope below) to carry the
                // timeframe key, so the issuesbyscope/attitudebyscope selectors'
                // date filter can reuse it without a signature change.
                cmd.Parameters.AddWithValue("@description", (object?)timeframe ?? DBNull.Value);
                cmd.AddMissingStoredProcedureParameters();

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    IssueScope i = new IssueScope();
                    i.Municipality = dr["municipality"].ToString();
                    i.Issue = dr["issue"].ToString();
                    i.Occurance = Convert.ToInt32(dr["occurance"]);

                    all.Add(i);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    public class IssueScope
    {
        public string? Municipality { get; set; }
        public string? Issue { get; set; }
        public int Occurance { get; set; }
    }

    [HttpGet]
    [Route("getattitudebyscope")]
    public IEnumerable<Report> getattitudebyscope([FromQuery] string? region, [FromQuery] string? unit, [FromQuery] string? ward, [FromQuery] string? vd, [FromQuery] string? timeframe)
    {
        MySqlDataReader dr;
        List<Report> all = new List<Report>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getReports", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@sender", "attitudebyscope");
                cmd.Parameters.AddWithValue("@region", (object?)region ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@municipality", (object?)unit ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ward", (object?)ward ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@vd", (object?)vd ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@description", (object?)timeframe ?? DBNull.Value);
                cmd.AddMissingStoredProcedureParameters();

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    r = new Report();
                    r.Municipality = dr["municipality"].ToString();
                    r.Total = dr["voters"].ToString();
                    r.Anc_supporters = dr["supporters"].ToString();
                    r.Undecided = dr["undecided"].ToString();
                    r.Non_supporters = dr["nonsupporters"].ToString();
                    r.Not_captured = dr["notcaptured"].ToString();

                    all.Add(r);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    [HttpGet]
    [Route("getprovincialstats")]
    public IEnumerable<Report> getprovincialstats()
    {
        MySqlDataReader dr;
        List<Report> all = new List<Report>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getReports", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@sender", "provincialnumbers");
                cmd.AddMissingStoredProcedureParameters();

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    r = new Report();
                    r.Total_1 = dr["Recorded"].ToString();
                    r.Total_2 = dr["AllVoters"].ToString();
                    r.Total_3 = dr["Users"].ToString();

                    all.Add(r);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    [HttpGet]
    [Route("getprovincialagedemographics")]
    public IEnumerable<AgeDemographics> getprovincialagedemographics()
    {
        MySqlDataReader dr;
        AgeDemographics r;
        List<AgeDemographics> all = new List<AgeDemographics>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getReports", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@sender", "getprovincialagedemographics");
                cmd.AddMissingStoredProcedureParameters();

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    r = new AgeDemographics();
                    r.voters = dr["voters"].ToString();
                    r.youth = dr["youth"].ToString();
                    r.between35_50 = dr["between_35_50"].ToString();
                    r.between50_65 = dr["between_50_65"].ToString();
                    r.over_65 = dr["over_65"].ToString();

                    all.Add(r);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    [HttpGet]
    [Route("getmunicipalagedemographics")]
    public IEnumerable<AgeDemographics> getmunicipalagedemographics()
    {
        MySqlDataReader dr;
        AgeDemographics r;
        List<AgeDemographics> all = new List<AgeDemographics>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getReports", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@sender", "getmunicipalagedemographics");
                cmd.AddMissingStoredProcedureParameters();

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    r = new AgeDemographics();
                    r.municipality = dr["municipality"].ToString();
                    r.voters = dr["voters"].ToString();
                    r.youth = dr["youth"].ToString();
                    r.between35_50 = dr["between_35_50"].ToString();
                    r.between50_65 = dr["between_50_65"].ToString();
                    r.over_65 = dr["over_65"].ToString();

                    all.Add(r);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    // The *byscope endpoints below take an optional `role` (e.g. "Youth League"):
    // when set, getReports counts only canvassing by volunteers whose cell number
    // matches a Users row with that role. Targets/registered voters stay unfiltered.
    [HttpGet]
    [Route("getprovincialnumbersbyscope")]
    public IEnumerable<Report> getprovincialnumbersbyscope([FromQuery] string? region, [FromQuery] string? unit, [FromQuery] string? ward, [FromQuery] string? vd, [FromQuery] string? role)
    {
        MySqlDataReader dr;
        List<Report> all = new List<Report>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getReports", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@sender", "provincialbyscope");
                cmd.Parameters.AddWithValue("@region", (object?)region ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@municipality", (object?)unit ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ward", (object?)ward ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@vd", (object?)vd ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@role", (object?)role ?? DBNull.Value);
                cmd.AddMissingStoredProcedureParameters();

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    r = new Report();
                    r.Municipality = dr["municipality"].ToString();
                    r.Special = dr["Special"].ToString();
                    r.Recorded = dr["Recorded"].ToString();
                    r.Total_1 = dr["Total_1"].ToString();

                    all.Add(r);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    // Daily canvassing counts per municipality / ward / VD from `from` (yyyy-MM-dd)
    // to today, for the dashboard's daily movement table. `municipalities` is a
    // comma-separated list of RecordedVoters.municipality codes (e.g.
    // "MAN - Mangaung" or "FS194 - Maluti a Phofung,FS192 - Dihlabeng"); the
    // frontend rolls VDs up into wards, clusters, zones and regions itself.
    [HttpGet]
    [Route("getdailycanvassing")]
    public IEnumerable<DailyCanvassing> getdailycanvassing([FromQuery] string? municipalities, [FromQuery] string? from)
    {
        List<DailyCanvassing> all = new List<DailyCanvassing>();

        var codes = (municipalities ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct()
            .Take(20)
            .ToList();
        if (codes.Count == 0)
        {
            return all;
        }

        // Default to the last 7 days; never look back more than 31 days.
        DateTime start = DateTime.TryParse(from, out var parsed) ? parsed.Date : DateTime.Today.AddDays(-6);
        if (start < DateTime.Today.AddDays(-31))
        {
            start = DateTime.Today.AddDays(-31);
        }

        var names = codes.Select((_, i) => "@m" + i).ToList();
        string sql =
            "SELECT municipality, ward, voting_district, DATE_FORMAT(`date`, '%Y-%m-%d') AS day, COUNT(names) AS recorded " +
            "FROM RecordedVoters " +
            "WHERE municipality IN (" + string.Join(", ", names) + ") AND `date` >= @from " +
            "GROUP BY municipality, ward, voting_district, DATE_FORMAT(`date`, '%Y-%m-%d')";

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(sql, con))
            {
                cmd.CommandType = CommandType.Text;
                for (int i = 0; i < codes.Count; i++)
                {
                    cmd.Parameters.AddWithValue(names[i], codes[i]);
                }
                cmd.Parameters.AddWithValue("@from", start);

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        all.Add(new DailyCanvassing
                        {
                            Municipality = dr["municipality"].ToString(),
                            Ward = dr["ward"].ToString(),
                            Vd = dr["voting_district"].ToString(),
                            Day = dr["day"].ToString(),
                            Recorded = Convert.ToInt32(dr["recorded"]),
                        });
                    }
                }
                con.Close();
            }
        }

        return all.ToArray();
    }

    // Every ward and VD on the voters' roll (AllVoters) for the given
    // municipalities, with the VD name — so the dashboard's Movement Report can
    // list areas with no canvassing yet. Same comma-separated `municipalities`
    // format as getdailycanvassing.
    [HttpGet]
    [Route("getvdroll")]
    public IEnumerable<DailyCanvassing> getvdroll([FromQuery] string? municipalities)
    {
        List<DailyCanvassing> all = new List<DailyCanvassing>();

        var codes = (municipalities ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct()
            .Take(20)
            .ToList();
        if (codes.Count == 0)
        {
            return all;
        }

        foreach (var code in codes)
        {
            all.AddRange(VdRollFor(code));
        }
        return all.ToArray();
    }

    // The roll barely changes, so each municipality's list is cached for 6 hours.
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, (DateTime At, List<DailyCanvassing> Rows)> vdRollCache = new();

    private List<DailyCanvassing> VdRollFor(string code)
    {
        if (vdRollCache.TryGetValue(code, out var hit) && hit.At > DateTime.UtcNow.AddHours(-6))
        {
            return hit.Rows;
        }

        // Filter on the ward-code prefix (indexed WardID) rather than the
        // Municipality column — ~7x faster on AllVoters. "FS194 - ..." -> "41904",
        // "MAN - Mangaung" -> "49400" (same prefix rule as the ward codes).
        string digits = new string(code.TakeWhile(c => c != ' ').Where(char.IsDigit).ToArray());
        string prefix = code.StartsWith("MAN", StringComparison.OrdinalIgnoreCase)
            ? "49400"
            : digits.Length == 3 ? "4" + digits.Substring(0, 2) + "0" + digits[2] : "";
        var rows = new List<DailyCanvassing>();
        if (prefix == "")
        {
            return rows;
        }

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(
                "SELECT WardID, VDNumber, MAX(VDName) AS VDName FROM AllVoters " +
                "WHERE WardID LIKE @prefix GROUP BY WardID, VDNumber", con))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@prefix", prefix + "%");
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        rows.Add(new DailyCanvassing
                        {
                            Municipality = code,
                            Ward = dr["WardID"].ToString(),
                            Vd = dr["VDNumber"].ToString(),
                            Name = dr["VDName"].ToString(),
                        });
                    }
                }
                con.Close();
            }
        }

        vdRollCache[code] = (DateTime.UtcNow, rows);
        return rows;
    }

    [HttpGet]
    [Route("getprovincialstatsbyscope")]
    public IEnumerable<Report> getprovincialstatsbyscope([FromQuery] string? region, [FromQuery] string? unit, [FromQuery] string? ward, [FromQuery] string? vd, [FromQuery] string? role)
    {
        MySqlDataReader dr;
        List<Report> all = new List<Report>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getReports", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@sender", "provincialstatsbyscope");
                cmd.Parameters.AddWithValue("@region", (object?)region ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@municipality", (object?)unit ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ward", (object?)ward ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@vd", (object?)vd ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@role", (object?)role ?? DBNull.Value);
                cmd.AddMissingStoredProcedureParameters();

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    r = new Report();
                    r.Total_1 = dr["Recorded"].ToString();
                    r.Total_2 = dr["AllVoters"].ToString();
                    r.Total_3 = dr["Users"].ToString();

                    all.Add(r);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    [HttpGet]
    [Route("getprovincialagedemographicsbyscope")]
    public IEnumerable<AgeDemographics> getprovincialagedemographicsbyscope([FromQuery] string? region, [FromQuery] string? unit, [FromQuery] string? ward, [FromQuery] string? vd, [FromQuery] string? role)
    {
        MySqlDataReader dr;
        AgeDemographics r;
        List<AgeDemographics> all = new List<AgeDemographics>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getReports", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@sender", "provincialagedemographicsbyscope");
                cmd.Parameters.AddWithValue("@region", (object?)region ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@municipality", (object?)unit ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ward", (object?)ward ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@vd", (object?)vd ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@role", (object?)role ?? DBNull.Value);
                cmd.AddMissingStoredProcedureParameters();

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    r = new AgeDemographics();
                    r.voters = dr["voters"].ToString();
                    r.youth = dr["youth"].ToString();
                    r.between35_50 = dr["between_35_50"].ToString();
                    r.between50_65 = dr["between_50_65"].ToString();
                    r.over_65 = dr["over_65"].ToString();

                    all.Add(r);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    [HttpGet]
    [Route("getmunicipalagedemographicsbyscope")]
    public IEnumerable<AgeDemographics> getmunicipalagedemographicsbyscope([FromQuery] string? region, [FromQuery] string? unit, [FromQuery] string? ward, [FromQuery] string? vd, [FromQuery] string? role)
    {
        MySqlDataReader dr;
        AgeDemographics r;
        List<AgeDemographics> all = new List<AgeDemographics>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getReports", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@sender", "municipalagedemographicsbyscope");
                cmd.Parameters.AddWithValue("@region", (object?)region ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@municipality", (object?)unit ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ward", (object?)ward ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@vd", (object?)vd ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@role", (object?)role ?? DBNull.Value);
                cmd.AddMissingStoredProcedureParameters();

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    r = new AgeDemographics();
                    r.municipality = dr["municipality"].ToString();
                    r.voters = dr["voters"].ToString();
                    r.youth = dr["youth"].ToString();
                    r.between35_50 = dr["between_35_50"].ToString();
                    r.between50_65 = dr["between_50_65"].ToString();
                    r.over_65 = dr["over_65"].ToString();

                    all.Add(r);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    [HttpGet]
    [Route("getwardlist")]
    public IEnumerable<WardListItem> getwardlist([FromQuery] string? region, [FromQuery] string? unit)
    {
        MySqlDataReader dr;
        List<WardListItem> all = new List<WardListItem>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getReports", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@sender", "wardlist");
                cmd.Parameters.AddWithValue("@region", (object?)region ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@municipality", (object?)unit ?? DBNull.Value);
                cmd.AddMissingStoredProcedureParameters();

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    all.Add(new WardListItem { ward = dr["WardID"].ToString() });
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    [HttpGet]
    [Route("getvdlist")]
    public IEnumerable<VdListItem> getvdlist([FromQuery] string? ward)
    {
        MySqlDataReader dr;
        List<VdListItem> all = new List<VdListItem>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getReports", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@sender", "vdlist");
                cmd.Parameters.AddWithValue("@ward", (object?)ward ?? DBNull.Value);
                cmd.AddMissingStoredProcedureParameters();

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    all.Add(new VdListItem
                    {
                        vdNumber = dr["VDNumber"].ToString(),
                        vdName = dr["VDName"].ToString(),
                    });
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    [HttpGet]
    [Route("getregionalnumbers")]
    public IEnumerable<Report> getregionalnumbers()
    {
        MySqlDataReader dr;
        Report r;
        List<Report> all = new List<Report>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRegionTotals", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddMissingStoredProcedureParameters();

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    r = new Report();
                    r.Municipality = dr["Region"].ToString();
                    r.Total = dr["Total"].ToString();
                    r.Total_1 = dr["Total_1"].ToString();
                    r.Total_2 = dr["Total_2"].ToString();
                    // r.Total_3 = dr["Total_3"].ToString();

                    all.Add(r);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    [HttpGet]
    [Route("getissues")]
    public async Task<ActionResult<IEnumerable<IssueCount>>> getissues()
    {
        List<IssueCount> issueCounts = new List<IssueCount>();

        using (MySqlConnection connection = new MySqlConnection(connect))
        {
            await connection.OpenAsync();

            string sqlQuery = @"
                WITH RECURSIVE split_issues AS (
                    SELECT
                        TRIM(SUBSTRING_INDEX(issuesArr, ',', 1)) AS issue,
                        CASE
                            WHEN INSTR(issuesArr, ',') > 0 
                            THEN SUBSTRING(issuesArr, INSTR(issuesArr, ',') + 1)
                            ELSE ''
                        END AS remaining
                    FROM RecordedVoters
                    WHERE issuesArr IS NOT NULL AND issuesArr <> ''

                    UNION ALL

                    SELECT
                        TRIM(SUBSTRING_INDEX(remaining, ',', 1)) AS issue,
                        CASE
                            WHEN INSTR(remaining, ',') > 0 
                            THEN SUBSTRING(remaining, INSTR(remaining, ',') + 1)
                            ELSE ''
                        END AS remaining
                    FROM split_issues
                    WHERE remaining <> ''
                )
                SELECT 
                    issue, 
                    COUNT(*) AS occurance,
                    (SELECT COUNT(*) FROM AllVoters) AS Total
                FROM split_issues
                WHERE issue <> ''
                GROUP BY issue
                HAVING COUNT(*) > 2
                ORDER BY COUNT(*) DESC;";

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        IssueCount issueCount = new IssueCount
                        {
                            Issue = reader["issue"].ToString(),
                            Occurance = Convert.ToInt32(reader["occurance"]),
                            Total = reader["Total"].ToString()
                        };

                        issueCounts.Add(issueCount);
                    }
                }
            }
        }
        return Ok(issueCounts);
    }
    public class IssueCount
    {
        public string? Issue { get; set; }
        public int Occurance { get; set; }
        public string? Total { get; set; }
    }
}