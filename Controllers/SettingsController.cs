using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using Web_Api.Models;

namespace Web_Api.Controllers;

[Route("[controller]")]
[ApiController]
public class SettingsController : ControllerBase
{
    private IConfiguration? _config;
    private Settings? u;
    private string? connect;

    public SettingsController(IConfiguration configuration)
    {
        _config = configuration;
        connect = _config.GetConnectionString("ConsString");
        u = new Settings();
    }
    //GET ALL USERS
    [HttpGet]
    public IEnumerable<Settings> getall()
    {
        MySqlDataReader dr;
        List<Settings> all = new List<Settings>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getSettings", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@selector", "get");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    u = new Settings();
                    u.id = Convert.ToInt32(dr["id"].ToString());
                    u.canvassing = dr["canvassing"].ToString();
                    u.election = dr["election"].ToString();
                    u.reports = dr["reports"].ToString();
                    u.users = dr["users"].ToString();
                    u.update = dr["update"].ToString();
                    u.apkUrl = dr["apkUrl"].ToString();
                    //
                    all.Add(u);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
}