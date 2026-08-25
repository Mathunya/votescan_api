using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace Web_Api.Controllers;

[Route("[controller]")]
[ApiController]
public class UserCountController : ControllerBase
{
    private IConfiguration _config;
    private string? connect;

    public UserCountController(IConfiguration configuration)
    {
        _config = configuration;
        connect = _config.GetConnectionString("ConsString");
    }

    //-------------------------------------------------------
    // GET REGIONAL COUNT
    //-------------------------------------------------------
    [HttpGet]
    [Route("getregionalcount")]
    public IEnumerable<UserCount> getregionalcount()
    {
        List<UserCount> all = new List<UserCount>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();

            using (MySqlCommand cmd = new MySqlCommand("getUserCount", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@selector", "regionalcount");
                cmd.AddMissingStoredProcedureParameters();

                using MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    all.Add(new UserCount
                    {
                        region = dr["Region"].ToString(),
                        total = Convert.ToInt32(dr["Total"])
                    });
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
    public IEnumerable<UserCount> getmunicipalcount(string region)
    {
        List<UserCount> all = new List<UserCount>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();

            using (MySqlCommand cmd = new MySqlCommand("getUserCount", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@region", region);
                cmd.Parameters.AddWithValue("@selector", "municipalcount");
                cmd.AddMissingStoredProcedureParameters();

                using MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    all.Add(new UserCount
                    {
                        municipality = dr["Municipality"].ToString(),
                        total = Convert.ToInt32(dr["Total"])
                    });
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
    public IEnumerable<UserCount> getwardcount(string municipality)
    {
        List<UserCount> all = new List<UserCount>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();

            using (MySqlCommand cmd = new MySqlCommand("getUserCount", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@municipality", municipality);
                cmd.Parameters.AddWithValue("@selector", "wardcount");
                cmd.AddMissingStoredProcedureParameters();

                using MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    all.Add(new UserCount
                    {
                        ward = dr["Ward"].ToString(),
                        total = Convert.ToInt32(dr["Total"])
                    });
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
    [Route("getvdcount")]
    public IEnumerable<UserCount> getvdcount(string ward)
    {
        List<UserCount> all = new List<UserCount>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();

            using (MySqlCommand cmd = new MySqlCommand("getUserCount", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ward", ward);
                cmd.Parameters.AddWithValue("@selector", "vdcount");
                cmd.AddMissingStoredProcedureParameters();

                using MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    all.Add(new UserCount
                    {
                        vdNumber = dr["VdNumber"].ToString(),
                        vdName = dr["VdName"].ToString(),
                        total = Convert.ToInt32(dr["Total"])
                    });
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }

    public class UserCount
    {
        public string? region { get; set; }
        public string? municipality { get; set; }
        public string? ward { get; set; }
        public string? vdName { get; set; }
        public string? vdNumber { get; set; }
        public int total { get; set; }
    }
}
