using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using Web_Api.Models;

namespace Web_Api.Controllers;

[Route("[controller]")]
[ApiController]
public class ElectionsController : ControllerBase
{
    private IConfiguration? _config;
    private Elections? e;
    private string? connect;

    public ElectionsController(IConfiguration configuration)
    {
        _config = configuration;
        connect = _config.GetConnectionString("ConsString");
        e = new Elections();
    }
    //GET ALL ELECTIONS
    [HttpGet]
    public IEnumerable<Elections> getall()
    {
        MySqlDataReader dr;
        List<Elections> all = new List<Elections>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getElections", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@selector", "all");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    e = new Elections();
                    e.id = Convert.ToInt32(dr["id"].ToString());
                    e.name = dr["name"].ToString();
                    e.desc = dr["description"].ToString();
                    e.start_date = dr["start_date"].ToString();
                    e.end_date = dr["end_date"].ToString();
                    e.status = dr["status"].ToString();
                    e.Ward = dr["Ward"].ToString();
                    //
                    all.Add(e);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    //GET ELECTIONS BY ID
    [HttpGet]
    [Route("byid/{id}")]
    public IEnumerable<Elections> byid(int id)
    {
        MySqlDataReader dr;
        List<Elections> all = new List<Elections>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getElections", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@selector", "byid");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    e = new Elections();
                    e.id = Convert.ToInt32(dr["id"].ToString());
                    e.name = dr["name"].ToString();
                    e.desc = dr["description"].ToString();
                    e.start_date = dr["start_date"].ToString();
                    e.end_date = dr["end_date"].ToString();
                    e.status = dr["status"].ToString();
                    e.Ward = dr["Ward"].ToString();
                    //
                    all.Add(e);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    //GET ELECTIONS BY NAME
    [HttpGet]
    [Route("byname/{name}")]
    public IEnumerable<Elections> byname(string name)
    {
        MySqlDataReader dr;
        List<Elections> all = new List<Elections>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getElections", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@selector", "byname");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    e = new Elections();
                    e.id = Convert.ToInt32(dr["id"].ToString());
                    e.name = dr["name"].ToString();
                    e.desc = dr["description"].ToString();
                   e.start_date = dr["start_date"].ToString();
                    e.end_date = dr["end_date"].ToString();
                    e.status = dr["status"].ToString();
                    e.Ward = dr["Ward"].ToString();
                    //
                    all.Add(e);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    //GET EACTIVE ELECTION
    [HttpGet]
    [Route("getactive")]
    public IEnumerable<Elections> getactive()
    {
        MySqlDataReader dr;
        List<Elections> all = new List<Elections>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getElections", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@selector", "getactive");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    e = new Elections();
                    e.id = Convert.ToInt32(dr["id"].ToString());
                    e.name = dr["name"].ToString();
                    e.desc = dr["description"].ToString();
                    e.start_date = dr["start_date"].ToString();
                    e.end_date = dr["end_date"].ToString();
                    e.status = dr["status"].ToString();
                    e.Ward = dr["Ward"].ToString();
                    //
                    all.Add(e);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }


}