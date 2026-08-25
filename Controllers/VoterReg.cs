using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using Web_Api.Models;

namespace Web_Api.Controllers;

[Route("[controller]")]
[ApiController]
public class VoterReg : ControllerBase
{
    private IConfiguration _config;
    private Voters v;
    private string? connect;

    public VoterReg(IConfiguration configuration)
    {
        _config = configuration;
        connect = _config.GetConnectionString("ConsString");
        v = new Voters();
    }

    // POST VOTER REGISTRATION
    [HttpPost("voterreg")]
    public async Task<IActionResult> voterreg([FromBody] VoterRegModel voter)
    {
        int retCode = 0;

        if (voter == null)
            return BadRequest("Request is null");

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            using (MySqlCommand cmd = new MySqlCommand("sp_VoterReg", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@selector", "add");
                cmd.Parameters.AddWithValue("@fullname", voter.fullName);
                cmd.Parameters.AddWithValue("@surname", voter.surname);
                cmd.Parameters.AddWithValue("@cell", voter.cell);
                cmd.Parameters.AddWithValue("@vdNumber", voter.vdNumber);
                cmd.Parameters.AddWithValue("@vdName", voter.vdName);
                cmd.Parameters.AddWithValue("@wardID", voter.wardID);
                cmd.Parameters.AddWithValue("@municipality", voter.municipality);
                cmd.Parameters.AddWithValue("@region", voter.region);
                cmd.Parameters.AddWithValue("@province", voter.province);
                cmd.Parameters.AddWithValue("@type", voter.type);

                con.Open();
                cmd.AddMissingStoredProcedureParameters();

                var ret = cmd.ExecuteNonQuery();
                retCode = Convert.ToInt32(ret);

                con.Close();
            }
        }

        return Ok(retCode);
    }

    // VIEW ALL
    [HttpGet("viewall")]
    public IActionResult ViewAll()
    {
        try
        {
            using MySqlConnection con = new MySqlConnection(connect);
            using MySqlCommand cmd = new MySqlCommand("sp_VoterReg", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@selector", "viewall");

            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            cmd.AddMissingStoredProcedureParameters();
            da.Fill(dt);

            return Ok(ToRows(dt));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // VIEW BY MUNICIPALITY
    [HttpGet("viewbymunicipality/{municipality}")]
    public IActionResult ViewByMunicipality(string municipality)
    {
        try
        {
            using MySqlConnection con = new MySqlConnection(connect);
            using MySqlCommand cmd = new MySqlCommand("sp_VoterReg", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@selector", "bymunicipality");
            cmd.Parameters.AddWithValue("@municipality", municipality);

            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            cmd.AddMissingStoredProcedureParameters();
            da.Fill(dt);

            return Ok(ToRows(dt));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // VIEW BY WARD
    [HttpGet("viewbyward/{wardID}")]
    public IActionResult ViewByWard(string wardID)
    {
        try
        {
            using MySqlConnection con = new MySqlConnection(connect);
            using MySqlCommand cmd = new MySqlCommand("sp_VoterReg", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@selector", "byward");
            cmd.Parameters.AddWithValue("@wardID", wardID);

            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            cmd.AddMissingStoredProcedureParameters();
            da.Fill(dt);

            return Ok(ToRows(dt));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // VIEW BY VD NUMBER
    [HttpGet("viewbyvdnumber/{vdNumber}")]
    public IActionResult ViewByVdNumber(string vdNumber)
    {
        try
        {
            using MySqlConnection con = new MySqlConnection(connect);
            using MySqlCommand cmd = new MySqlCommand("sp_VoterReg", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@selector", "byvdnumber");
            cmd.Parameters.AddWithValue("@vdNumber", vdNumber);

            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            cmd.AddMissingStoredProcedureParameters();
            da.Fill(dt);

            return Ok(ToRows(dt));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    //-------------------------------------------------------
    // GET VOTER REGISTRATIONS
    //-------------------------------------------------------

    // GET VOTER
    [HttpGet("getvoterreg")]
    public IActionResult GetVoterReg()
    {
        try
        {
            using MySqlConnection con = new MySqlConnection(connect);
            using MySqlCommand cmd = new MySqlCommand("getElections", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@selector", "getvoterreg");

            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            cmd.AddMissingStoredProcedureParameters();
            da.Fill(dt);

            return Ok(ToRows(dt));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //-------------------------------------------------------
    // GET COUNT
    //-------------------------------------------------------
    [HttpGet]
    [Route("getcount")]
    public IEnumerable<VoterRegCount> getcount()
    {
        VoterRegCount num = new VoterRegCount();
        List<VoterRegCount> all = new List<VoterRegCount>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();

            using (MySqlCommand cmd = new MySqlCommand("sp_VoterReg", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@selector", "count");
                cmd.AddMissingStoredProcedureParameters();

                using MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    num = new VoterRegCount();

                    num.newcheck = Convert.ToInt32(dr["NewVoters"].ToString());
                    num.recheck = Convert.ToInt32(dr["VoterReCheck"].ToString());

                    all.Add(num);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    //-------------------------------------------------------
    // GET REGIONAL COUNT
    //-------------------------------------------------------
    [HttpGet]
    [Route("getregionalcount")]
    public IEnumerable<VoterRegCount> getregionalcount(string startDate, string endDate)
    {
        VoterRegCount num = new VoterRegCount();
        List<VoterRegCount> all = new List<VoterRegCount>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();

            using (MySqlCommand cmd = new MySqlCommand("sp_VoterReg", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@selector", "regionalcount");
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);
                cmd.AddMissingStoredProcedureParameters();

                using MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    num = new VoterRegCount();

                    num.region = dr["Region"].ToString();
                    num.total1 = Convert.ToInt32(dr["Total"].ToString());
                    num.total0 = Convert.ToInt32(dr["Total0"].ToString());

                    all.Add(num);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    //-------------------------------------------------------
    // GET MUNICIPAL COUNT
    //-------------------------------------------------------
    [HttpGet]
    [Route("getmunicipalcount")]
    public IEnumerable<VoterRegCount> getmunicipalcount(string region, string startDate, string endDate)
    {
        VoterRegCount num = new VoterRegCount();
        List<VoterRegCount> all = new List<VoterRegCount>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();

            using (MySqlCommand cmd = new MySqlCommand("sp_VoterReg", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@region", region);
                cmd.Parameters.AddWithValue("@selector", "municipalcount");
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);
                cmd.AddMissingStoredProcedureParameters();

                using MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    num = new VoterRegCount();

                    num.municipality = dr["Municipality"].ToString();
                    num.total1 = Convert.ToInt32(dr["Total"].ToString());
                    num.total0 = Convert.ToInt32(dr["Total0"].ToString());

                    all.Add(num);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    //-------------------------------------------------------
    // GET WARD COUNT
    //-------------------------------------------------------
    [HttpGet]
    [Route("getwardcount")]
    public IEnumerable<VoterRegCount> getwardcount(string municipality, string startDate, string endDate)
    {
        VoterRegCount num = new VoterRegCount();
        List<VoterRegCount> all = new List<VoterRegCount>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();

            using (MySqlCommand cmd = new MySqlCommand("sp_VoterReg", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@municipality", municipality);
                cmd.Parameters.AddWithValue("@selector", "wardcount");
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);
                cmd.AddMissingStoredProcedureParameters();

                using MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    num = new VoterRegCount();

                    num.municipality = dr["WardID"].ToString();
                    num.total1 = Convert.ToInt32(dr["Total"].ToString());
                    num.total0 = Convert.ToInt32(dr["Total0"].ToString());

                    all.Add(num);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    //-------------------------------------------------------
    // GET VD COUNT
    //-------------------------------------------------------
    [HttpGet]
    [Route("getVDcount")]
    public IEnumerable<VoterRegCount> getVDcount(string ward, string startDate, string endDate)
    {
        VoterRegCount num = new VoterRegCount();
        List<VoterRegCount> all = new List<VoterRegCount>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();

            using (MySqlCommand cmd = new MySqlCommand("sp_VoterReg", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@wardID", ward);
                cmd.Parameters.AddWithValue("@selector", "vdcount");
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);
                cmd.AddMissingStoredProcedureParameters();

                using MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    num = new VoterRegCount();

                    num.vdName = dr["VdName"].ToString();
                    num.vdNumber = dr["VdNumber"].ToString();
                    num.total1 = Convert.ToInt32(dr["Total"].ToString());
                    num.total0 = Convert.ToInt32(dr["Total0"].ToString());

                    all.Add(num);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    private static List<Dictionary<string, object?>> ToRows(DataTable table)
    {
        var rows = new List<Dictionary<string, object?>>();

        foreach (DataRow row in table.Rows)
        {
            var item = new Dictionary<string, object?>();

            foreach (DataColumn column in table.Columns)
            {
                item[column.ColumnName] = row[column] == DBNull.Value ? null : row[column];
            }

            rows.Add(item);
        }

        return rows;
    }

    public class VoterRegCount
    {
        public string? region { get; set; }
        public string? municipality { get; set; }
        public string? ward { get; set; }
        public string? vdName { get; set; }
        public string? vdNumber { get; set; }
        public int? newcheck { get; set; }
        public int? recheck { get; set; }
        public int? total1 { get; set; } //todays total
        public int? total0 { get; set; } //yesterdays total
    }
}