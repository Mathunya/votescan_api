using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using Web_Api.Models;

namespace Web_Api.Controllers;

[Route("[controller]")]
[ApiController]

public class AttendantController : ControllerBase{

    private IConfiguration? _config;
    private Attendants? a;
    private string? connect;
    private MySqlDataAdapter? da;
    private DataSet? ds;

    public AttendantController(IConfiguration configuration)
    {
        _config = configuration;
        connect = _config.GetConnectionString("ConsString");
        a = new Attendants();
    }

     //GET ALL USERS
    [HttpGet]

    public IEnumerable<Attendants> getall()
    {
        MySqlDataReader dr;
        List<Attendants> all = new List<Attendants>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getEventsAttendants", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Selector", "all");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    a = new Attendants();
                    a.Id = dr["Id"].ToString();
                    a.Name = dr["Name"].ToString();
                    a.Surname = dr["Surname"].ToString();
                    a.Cell = dr["Cell"].ToString();
                    a.Ward = dr["Ward"].ToString();
                    a.VD = dr["VD"].ToString();
                    a.Event = dr["Event"].ToString();
                   
                    //
                    all.Add(a);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    [HttpPost]
    public int addUser(Attendants attendant)
    {
        var retCode = 0;
        // Add the new product to the database
        // ...
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getEventsAttendants", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", attendant.Name);
                cmd.Parameters.AddWithValue("@Surname", attendant.Surname);
                cmd.Parameters.AddWithValue("@Cell", attendant.Cell);
                cmd.Parameters.AddWithValue("@Ward", attendant.Ward);
                cmd.Parameters.AddWithValue("@VD", attendant.VD);
                cmd.Parameters.AddWithValue("@Event", attendant.Event);
                cmd.Parameters.AddWithValue("@Selector", "add");
                //
                //con.Open();
                da = new MySqlDataAdapter();
                ds = new DataSet();
                da.InsertCommand = cmd;
                da.InsertCommand.AddMissingStoredProcedureParameters();
                retCode = da.InsertCommand.ExecuteNonQuery();
                con.Close();
            }
        }
        //return codes

        //  -1 =failue
        //   1 = account saved successfully
        return retCode;
    }

    [HttpGet]
    [Route("getbyward/{ward}")]

    public IEnumerable<Attendants> getbyward(string ward)
    {
        MySqlDataReader dr;
        List<Attendants> all = new List<Attendants>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getEventsAttendants", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ward", ward);
                cmd.Parameters.AddWithValue("@Selector", "byward");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    a = new Attendants();
                    a.Id = dr["Id"].ToString();
                    a.Name = dr["Name"].ToString();
                    a.Surname = dr["Surname"].ToString();
                    a.Cell = dr["Cell"].ToString();
                    a.Ward = dr["Ward"].ToString();
                    a.VD = dr["VD"].ToString();
                    a.Event = dr["Event"].ToString();
                   
                    //
                    all.Add(a);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }

     [HttpGet]
    [Route("getbyevent/{events}")]

    public IEnumerable<Attendants> getbyevent(string events)
    {
        MySqlDataReader dr;
        List<Attendants> all = new List<Attendants>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getEventsAttendants", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Event", events);
                cmd.Parameters.AddWithValue("@Selector", "byevents");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    a = new Attendants();
                    a.Id = dr["Id"].ToString();
                    a.Name = dr["Name"].ToString();
                    a.Surname = dr["Surname"].ToString();
                    a.Cell = dr["Cell"].ToString();
                    a.Ward = dr["Ward"].ToString();
                    a.VD = dr["VD"].ToString();
                    a.Event = dr["Event"].ToString();
                   
                    //
                    all.Add(a);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }

     
   

}