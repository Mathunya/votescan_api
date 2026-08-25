using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using Web_Api.Models;
using System.Web;

namespace Web_Api.Controllers;

[Route("[controller]")]
[ApiController]
public class VotersController : ControllerBase
{
    private IConfiguration _config;
    private Voters v;
    private string? connect;
    private MySqlDataAdapter? da;
    private DataSet? ds;

    public VotersController(IConfiguration configuration)
    {
        _config = configuration;
        connect = _config.GetConnectionString("ConsString");
        v = new Voters();
    }
    //GET ALL VOTERS
    [HttpGet]
    public IEnumerable<Voters> getall()
    {
        MySqlDataReader dr;
        List<Voters> all = new List<Voters>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@selector", "all");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new Voters();
                    v.Id = dr["No"].ToString();
                    v.Surname = dr["Surname"].ToString();
                    v.Names = dr["Names"].ToString();
                    v.Address = dr["Address"].ToString();
                    v.Id_Number = dr["ID_Number"].ToString();
                    v.Province = dr["Province"].ToString();
                    v.Municipality = dr["Municipality"].ToString();
                    v.Voting_Station = dr["Voting_Station"].ToString();
                    v.Ward = dr["Ward"].ToString();
                    v.VOTING_DISTRICT = dr["VOTING_DISTRICT"].ToString();
                    v.Status = dr["Status"].ToString();
                    v.Date = dr["Date"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    //GET ALL VOTERS BY ID NUMBER
    [HttpGet]
    [Route("getbyid/{id_number}")]
    public IEnumerable<Voters> getbyid(string id_number)
    {
        MySqlDataReader dr;
        List<Voters> all = new List<Voters>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("id_number", id_number);
                cmd.Parameters.AddWithValue("@selector", "byid");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new Voters();
                    v.Id = dr["No"].ToString();
                    v.Surname = dr["Surname"].ToString();
                    v.Names = dr["Names"].ToString();
                    v.Address = dr["Address"].ToString();
                    v.Id_Number = dr["ID_Number"].ToString();
                    v.Province = dr["Province"].ToString();
                    v.Municipality = dr["Municipality"].ToString();
                    v.Voting_Station = dr["Voting_Station"].ToString();
                    v.Ward = dr["Ward"].ToString();
                    v.VOTING_DISTRICT = dr["VOTING_DISTRICT"].ToString();
                    v.Status = dr["Status"].ToString();
                    v.Date = dr["Date"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    [HttpPut]
    public int update(int number)
    {
        var retCode = 0;
        // Add the new product to the database
        // ...
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@No", number);
                cmd.Parameters.AddWithValue("@selector", "frontdesk");
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
    [Route("getrecordedfrontdesk")]
    public IEnumerable<Voters> getrecordedfrontdesk()
    {
        MySqlDataReader dr;
        List<Voters> all = new List<Voters>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@selector", "getrecordedfrontdesk");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new Voters();
                    v.Id = dr["No"].ToString();
                    v.Surname = dr["Surname"].ToString();
                    v.Names = dr["Names"].ToString();
                    v.Address = dr["Address"].ToString();
                    v.Id_Number = dr["ID_Number"].ToString();
                    v.Province = dr["Province"].ToString();
                    v.Municipality = dr["Municipality"].ToString();
                    v.Voting_Station = dr["Voting_Station"].ToString();
                    v.Ward = dr["Ward"].ToString();
                    v.VOTING_DISTRICT = dr["Voting_Station"].ToString();
                    v.Status = dr["Status"].ToString();
                    v.Date = dr["Date"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    [HttpGet]
    [Route("getbyward/{ward}")]
    public IEnumerable<Voters> getbyward(string ward)
    {
        MySqlDataReader dr;
        List<Voters> all = new List<Voters>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ward", ward);
                cmd.Parameters.AddWithValue("@selector", "getbyward");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new Voters();
                    v.Id = dr["No"].ToString();
                    v.Surname = dr["Surname"].ToString();
                    v.Names = dr["Names"].ToString();
                    v.Address = dr["Address"].ToString();
                    v.Id_Number = dr["ID_Number"].ToString();
                    v.Province = dr["Province"].ToString();
                    v.Municipality = dr["Municipality"].ToString();
                    v.Voting_Station = dr["Voting_Station"].ToString();
                    v.Ward = dr["Ward"].ToString();
                    v.VOTING_DISTRICT = dr["Voting_Station"].ToString();
                    v.Status = dr["Status"].ToString();
                    v.Date = dr["Date"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    [HttpGet]
    [Route("getwardnumbers")]
    public Numbers getwardnumbers()
    {
        MySqlDataReader dr;
        Numbers num = new Numbers();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            string[] allReturned = new string[] { };
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@selector", "wardnumbers");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    //wardnumbers
                    num = new Numbers();
                    num.recorded07 = dr["recorded07"].ToString();
                    num.recorded29 = dr["recorded29"].ToString();
                    num.recorded49 = dr["recorded49"].ToString();
                    num.recorded50 = dr["recorded50"].ToString();
                    num.wardO7 = dr["wardO7"].ToString();
                    num.ward29 = dr["ward29"].ToString();
                    num.ward49 = dr["ward49"].ToString();
                    num.ward50 = dr["ward50"].ToString();
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return num;
    }
    [HttpGet]
    [Route("getvdnumbers/{ward}")]
    public List<Numbers> getvdnumbers(string ward)
    {
        MySqlDataReader dr;
        Numbers num = new Numbers();
        List<Numbers> all = new List<Numbers>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            string[] allReturned = new string[] { };
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ward", ward);
                cmd.Parameters.AddWithValue("@selector", "vdnumbers");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    //wardnumbers
                    num = new Numbers();
                    num.voting_station = dr["Voting_Station"].ToString();
                    num.total = dr["Total"].ToString();
                    num.recorded07 = dr["Scanned"].ToString();
                    all.Add(num);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all;
    }
    [HttpGet]
    [Route("getstreets/{vs}")]
    public List<Numbers> getstreets(string vs)
    {
        MySqlDataReader dr;
        Numbers num = new Numbers();
        List<Numbers> all = new List<Numbers>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            string[] allReturned = new string[] { };
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@voting_station", vs);
                cmd.Parameters.AddWithValue("@selector", "getstreets");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    //wardnumbers
                    num = new Numbers();
                    num.streetName = dr["StreetName"].ToString();
                    num.total = dr["PeopleCount"].ToString();
                    all.Add(num);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all;
    }
    [HttpGet]
    [Route("getbystreetname/{street}")]
    public List<Voters> getbystreetname(string street)
    {
        MySqlDataReader dr;
        List<Voters> all = new List<Voters>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@street", street);
                cmd.Parameters.AddWithValue("@selector", "getbystreetname");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    //wardnumbers
                    v = new Voters();
                    v.Id = dr["No"].ToString();
                    v.Surname = dr["Surname"].ToString();
                    v.Names = dr["Names"].ToString();
                    v.Address = dr["Address"].ToString();
                    v.Id_Number = dr["ID_Number"].ToString();
                    v.Province = dr["Province"].ToString();
                    v.Municipality = dr["Municipality"].ToString();
                    v.Voting_Station = dr["Voting_Station"].ToString();
                    v.Ward = dr["Ward"].ToString();
                    v.VOTING_DISTRICT = dr["Voting_Station"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all;
    }
    [HttpGet]
    [Route("getwards")]
    public List<Voters> getwards()
    {
        MySqlDataReader dr;
        List<Voters> all = new List<Voters>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@selector", "wards");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    //wardnumbers
                    //Ward,Province,Municipality
                    v = new Voters();
                    v.Ward = dr["Ward"].ToString();
                    v.Province = dr["Province"].ToString();
                    v.Municipality = dr["Municipality"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all;
    }
    [HttpGet]
    [Route("getvds/{ward}")]
    public List<Voters> getvds(int ward)
    {
        MySqlDataReader dr;
        List<Voters> all = new List<Voters>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ward", ward);
                cmd.Parameters.AddWithValue("@selector", "vds");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    //wardnumbers
                    //Ward,Province,Municipality
                    v = new Voters();
                    v.VOTING_DISTRICT = dr["Voting_District"].ToString();
                    v.Voting_Station = dr["Voting_Station"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all;
    }


}
