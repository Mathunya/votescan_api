using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using Web_Api.Models;

namespace Web_Api.Controllers;

[Route("[controller]")]
[ApiController]
public class NewVotersController : ControllerBase
{
    private IConfiguration? _config;
    private NewVoter? u;
    private string? connect;
    private MySqlDataAdapter? da;
    private DataSet? ds;

    public NewVotersController(IConfiguration configuration)
    {
        _config = configuration;
        connect = _config.GetConnectionString("ConsString");
        u = new NewVoter();
    }
    //GET ALL NEW VOTERS
    [HttpGet]
    public IEnumerable<NewVoter> getall()
    {
        MySqlDataReader dr;
        List<NewVoter> all = new List<NewVoter>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getNewVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Selector", "all");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    u = new NewVoter();
                    u.Id = dr["Id"].ToString();
                    u.Name = dr["Name"].ToString();
                    u.Surname = dr["Surname"].ToString();
                    u.Cell = dr["Cell"].ToString();
                    u.DOB = dr["DOB"].ToString();
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
    //GET NEW VOTERS BY VOLUNTEER
    [HttpGet]
    [Route("getbyvolunteer/{volunteer}")]
    public IEnumerable<NewVoter> getbyvolunteer(string volunteer)
    {
        MySqlDataReader dr;
        List<NewVoter> all = new List<NewVoter>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getNewVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Volunteer", volunteer);
                cmd.Parameters.AddWithValue("@Selector", "byvolunteer");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    u = new NewVoter();
                     u.Id = dr["Id"].ToString();
                    u.Name = dr["Name"].ToString();
                    u.Surname = dr["Surname"].ToString();
                    u.Cell = dr["Cell"].ToString();
                    u.DOB = dr["DOB"].ToString();
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
    [HttpPost]
    public int addNewVoter(NewVoter user)
    {
        var retCode = 0;
        // Add the new product to the database
        // ...
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getNewVoters", con))
            {
                //Name,Surname,Cell,Ward,VD,Address,Volunteer,DOB
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Surname", user.Surname);
                cmd.Parameters.AddWithValue("@Cell", user.Cell);
                cmd.Parameters.AddWithValue("@Ward", user.Ward);
                cmd.Parameters.AddWithValue("@VD", user.VD);
                cmd.Parameters.AddWithValue("@Address", user.Address);
                cmd.Parameters.AddWithValue("@Volunteer", user.Volunteer);
                cmd.Parameters.AddWithValue("@DOB", user.DOB);
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
}