using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using Web_Api.Models;

namespace Web_Api.Controllers;

[Route("[controller]")]
[ApiController]
public class ReportController2024 : ControllerBase
{
    private IConfiguration? _config;
    private Report? r;
    private string? connect;

    public ReportController2024(IConfiguration configuration)
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
            using (MySqlCommand cmd = new MySqlCommand("getReports2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_sender", "provincial");
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
            using (MySqlCommand cmd = new MySqlCommand("getReports2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_sender", "attitude");
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
            using (MySqlCommand cmd = new MySqlCommand("getReports2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_region", region);
                cmd.Parameters.AddWithValue("@p_sender", "byregion");
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
            using (MySqlCommand cmd = new MySqlCommand("getReports2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_sender", "provincialnumbers");
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
            using (MySqlCommand cmd = new MySqlCommand("getReports2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_sender", "getprovincialagedemographics");
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
            using (MySqlCommand cmd = new MySqlCommand("getReports2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_sender", "getmunicipalagedemographics");
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
            using (MySqlCommand cmd = new MySqlCommand("getRegionTotals2024", con))
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
    public IEnumerable<IssueCount> getissues()
    {
        MySqlDataReader dr;
        List<IssueCount> all = new List<IssueCount>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getReports2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_sender", "issues");
                cmd.AddMissingStoredProcedureParameters();

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    all.Add(new IssueCount
                    {
                        Issue = dr["issue"].ToString(),
                        Occurance = Convert.ToInt32(dr["occurance"]),
                        Total = dr["Total_1"].ToString()
                    });
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    public class IssueCount
    {
        public string? Issue { get; set; }
        public int Occurance { get; set; }
        public string? Total { get; set; }
    }
}
