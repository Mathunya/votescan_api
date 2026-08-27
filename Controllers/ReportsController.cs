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

    [HttpGet]
    [Route("getprovincialnumbersbyscope")]
    public IEnumerable<Report> getprovincialnumbersbyscope([FromQuery] string? region, [FromQuery] string? unit)
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
    [Route("getprovincialstatsbyscope")]
    public IEnumerable<Report> getprovincialstatsbyscope([FromQuery] string? region, [FromQuery] string? unit)
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
    public IEnumerable<AgeDemographics> getprovincialagedemographicsbyscope([FromQuery] string? region, [FromQuery] string? unit)
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
    public IEnumerable<AgeDemographics> getmunicipalagedemographicsbyscope([FromQuery] string? region, [FromQuery] string? unit)
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