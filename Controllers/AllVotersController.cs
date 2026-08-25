using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using Web_Api.Models;
//OLD IP 156.38.171.132

namespace Web_Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AllVotersController : ControllerBase
{
    private IConfiguration _config;
    private AllVoters? v;
    private string? connect;

    public AllVotersController(IConfiguration configuration)
    {
        _config = configuration;
        connect = _config.GetConnectionString("ConsString");
        v = new AllVoters();
    }
    //GET ALL VOTERS IN THE WARD BY ID NUMBER 
    [HttpGet]
    [Route("getbyid/{id}")]
    public async Task<IEnumerable<AllVoters>> getbyid(string id)
    {
        List<AllVoters> all = new List<AllVoters>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            v = new AllVoters();
            using (MySqlCommand cmd = new MySqlCommand("getAllVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IDNumber", id);
                cmd.Parameters.AddWithValue("@selector", "byid");
                AddMissingGetAllVotersParameters(cmd);
                //open reader
                //dr = cmd.ExecuteReader();
                cmd.AddMissingStoredProcedureParameters();
                using(var dr= await cmd.ExecuteReaderAsync())
                {
                    //fill data in datatable
                    while (await dr.ReadAsync())
                    {
                        all.Add(new AllVoters
                        {
                            Province = dr["Province"].ToString(),
                            Municipality = dr["Municipality"].ToString(),
                            WardID = dr["WardID"].ToString(),
                            VDNumber = dr["VDNumber"].ToString(),
                            VDName = dr["VDName"].ToString(),
                            IDNumber = dr["IDNumber"].ToString(),
                            Surname = dr["Surname"].ToString(),
                            FullName = dr["FullName"].ToString(),
                            Voter_Status = dr["Voter Status"].ToString(),
                            VR_Address = dr["VR_Address"].ToString(),
                            StreetNumber = dr["StreetNumber"].ToString(),
                            StreetName = dr["StreetName"].ToString(),
                            Suburb = dr["Suburb"].ToString(),
                            Town = dr["Town"].ToString(),
                            ResPostalCode = dr["ResPostalCode"].ToString(),
                            VoterID = dr["VoterID"].ToString(),
                            ID = dr["ID"].ToString(),
                            Recorded = dr["Recorded"].ToString()
                    });
                    }
                    dr.Close();
                }
                //close connections
                
                con.Close();
            }
        }
        return all.ToArray();
    }
    //GET ALL VOTERS BY ID AND WARD
    [HttpGet]
    [Route("getbyward/{id}/{ward}")]
    public IEnumerable<AllVoters> getbyward(string id,string ward)
    {
        MySqlDataReader dr;
        List<AllVoters> all = new List<AllVoters>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getAllVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IDNumber", id);
                 cmd.Parameters.AddWithValue("@WardID", ward);
                cmd.Parameters.AddWithValue("@selector", "byidandward");
                AddMissingGetAllVotersParameters(cmd);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new AllVoters();
                    v.Province = dr["Province"].ToString();
                    v.Municipality = dr["Municipality"].ToString();
                    v.WardID = dr["WardID"].ToString();
                    v.VDNumber = dr["VDNumber"].ToString();
                    v.VDName = dr["VDName"].ToString();
                    v.IDNumber = dr["IDNumber"].ToString();
                    v.Surname = dr["Surname"].ToString();
                    v.FullName = dr["FullName"].ToString();
                    v.Voter_Status = dr["Voter Status"].ToString();
                    v.VR_Address = dr["VR_Address"].ToString();
                    v.StreetNumber = dr["StreetNumber"].ToString();
                    v.StreetName = dr["StreetName"].ToString();
                    v.Suburb = dr["Suburb"].ToString();
                    v.Town = dr["Town"].ToString();
                    v.ResPostalCode = dr["ResPostalCode"].ToString();
                    v.VoterID = dr["VoterID"].ToString();
                    v.ID = dr["ID"].ToString();
                    v.Recorded = dr["Recorded"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
     //GET ALL VOTERS BY WARD NUMBER
    [HttpGet]
    [Route("getbywardnumber/{ward}")]
    public IEnumerable<AllVoters> getbywardnumber(string ward)
    {
        MySqlDataReader dr;
        List<AllVoters> all = new List<AllVoters>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getAllVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                 cmd.Parameters.AddWithValue("@WardID", ward);
                cmd.Parameters.AddWithValue("@selector", "getbyward");
                AddMissingGetAllVotersParameters(cmd);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new AllVoters();
                    v.Province = dr["Province"].ToString();
                    v.Municipality = dr["Municipality"].ToString();
                    v.WardID = dr["WardID"].ToString();
                    v.VDNumber = dr["VDNumber"].ToString();
                    v.VDName = dr["VDName"].ToString();
                    v.IDNumber = dr["IDNumber"].ToString();
                    v.Surname = dr["Surname"].ToString();
                    v.FullName = dr["FullName"].ToString();
                    v.VR_Address = dr["VR_Address"].ToString();
                    v.StreetNumber = dr["StreetNumber"].ToString();
                    v.StreetName = dr["StreetName"].ToString();
                    v.Suburb = dr["Suburb"].ToString();
                    v.Town = dr["Town"].ToString();
                    v.ResPostalCode = dr["ResPostalCode"].ToString();
                    v.VoterID = dr["VoterID"].ToString();
                    v.ID = dr["ID"].ToString();
                    v.Recorded = dr["Recorded"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    //GET BY VD NUMBER
    [HttpGet]
    [Route("getbyvd/{vd}")]
    public IEnumerable<AllVoters> getbyvd(string vd)
    {
        MySqlDataReader dr;
        List<AllVoters> all = new List<AllVoters>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getAllVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                 cmd.Parameters.AddWithValue("@VDNumber", vd);
                cmd.Parameters.AddWithValue("@selector", "getbyvd");
                AddMissingGetAllVotersParameters(cmd);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new AllVoters();
                    v.Province = dr["Province"].ToString();
                    v.Municipality = dr["Municipality"].ToString();
                    v.WardID = dr["WardID"].ToString();
                    v.VDNumber = dr["VDNumber"].ToString();
                    v.VDName = dr["VDName"].ToString();
                    v.IDNumber = dr["IDNumber"].ToString();
                    v.Surname = dr["Surname"].ToString();
                    v.FullName = dr["FullName"].ToString();
                    v.VR_Address = dr["VR_Address"].ToString();
                    v.StreetNumber = dr["StreetNumber"].ToString();
                    v.StreetName = dr["StreetName"].ToString();
                    v.Suburb = dr["Suburb"].ToString();
                    v.Town = dr["Town"].ToString();
                    v.ResPostalCode = dr["ResPostalCode"].ToString();
                    v.VoterID = dr["VoterID"].ToString();
                    v.ID = dr["ID"].ToString();
                    v.Recorded = dr["Recorded"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    //GET ALL STREEET NAMES  BY VD NUMBER
    [HttpGet]
    [Route("getvdstreets/{vd}")]
    public IEnumerable<AllVoters> getvdstreets(string vd)
    {
        MySqlDataReader dr;
        List<AllVoters> all = new List<AllVoters>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getAllVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                 cmd.Parameters.AddWithValue("@VDNumber", vd);
                cmd.Parameters.AddWithValue("@selector", "getvdstreets");
                AddMissingGetAllVotersParameters(cmd);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new AllVoters();
                    v.Suburb = dr["Suburb"].ToString();
                    v.VDNumber = dr["VDNumber"].ToString();
                   
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
     //GET ALL USERS BY SUBURB AND VD NUMBER
    [HttpGet]
    [Route("getbystreetandvd/{vd}/{suburb}")]
    public IEnumerable<AllVoters> getbystreetandvd(string vd,string suburb)
    {
        MySqlDataReader dr;
        List<AllVoters> all = new List<AllVoters>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getAllVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VDNumber", vd);
                cmd.Parameters.AddWithValue("@Suburb", suburb);
                cmd.Parameters.AddWithValue("@selector", "getbystreetandvd");
                AddMissingGetAllVotersParameters(cmd);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new AllVoters();
                    v.Province = dr["Province"].ToString();
                    v.Municipality = dr["Municipality"].ToString();
                    v.WardID = dr["WardID"].ToString();
                    v.VDNumber = dr["VDNumber"].ToString();
                    v.VDName = dr["VDName"].ToString();
                    v.IDNumber = dr["IDNumber"].ToString();
                    v.Surname = dr["Surname"].ToString();
                    v.FullName = dr["FullName"].ToString();
                    v.VR_Address = dr["VR_Address"].ToString();
                    v.StreetNumber = dr["StreetNumber"].ToString();
                    v.StreetName = dr["StreetName"].ToString();
                    v.Suburb = dr["Suburb"].ToString();
                    v.Town = dr["Town"].ToString();
                    v.ResPostalCode = dr["ResPostalCode"].ToString();
                    v.VoterID = dr["VoterID"].ToString();
                    v.ID = dr["ID"].ToString();
                    v.Recorded = dr["Recorded"].ToString();
                    //
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    //GET ALL MUNICIPALITIES
    [HttpGet]
    [Route("getmunicipalities")]
    public IEnumerable<AllVoters> getmunicipalities()
    {
        MySqlDataReader dr;
        List<AllVoters> all = new List<AllVoters>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getAllVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@selector", "getmunicipalities");
                AddMissingGetAllVotersParameters(cmd);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new AllVoters();
                    v.Municipality = dr["Municipality"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    //GET ALL WARDS BY MUNICIPALITY
    [HttpGet]
    [Route("getwards/{municipality}")]
    public IEnumerable<AllVoters> getwards(string municipality)
    {
        MySqlDataReader dr;
        List<AllVoters> all = new List<AllVoters>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getAllVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Municipality", municipality);
                cmd.Parameters.AddWithValue("@selector", "getwards");
                AddMissingGetAllVotersParameters(cmd);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new AllVoters();
                    v.WardID = dr["WardID"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
     //GET ALL VDS BY WARD
    [HttpGet]
    [Route("getvds/{ward}")]
    public IEnumerable<AllVoters> getvds(string ward)
    {
        MySqlDataReader dr;
        List<AllVoters> all = new List<AllVoters>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getAllVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WardID", ward);
                cmd.Parameters.AddWithValue("@selector", "getvds");
                AddMissingGetAllVotersParameters(cmd);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new AllVoters();
                    v.VDNumber = dr["VDNumber"].ToString();
                    v.VDName = dr["VDName"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
     //GET ALL VDS BY WARD
    [HttpGet]
    [Route("getmunibyward/{ward}")]
    public IEnumerable<AllVoters> getvgetmunibywardds(string ward)
    {
        MySqlDataReader dr;
        List<AllVoters> all = new List<AllVoters>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getAllVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WardID", ward);
                cmd.Parameters.AddWithValue("@selector", "getmunibyward");
                AddMissingGetAllVotersParameters(cmd);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new AllVoters();
                    v.Municipality = dr["Municipality"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    //GET BY VD NUMBER
    [HttpGet]
    [Route("getVoter/{id}/{surname}/{names}")]
    public IEnumerable<AllVoters>getVoter (string id,string surname, string names)
    {
        MySqlDataReader dr;
        List<AllVoters> all = new List<AllVoters>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getAllVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IDNumber", id);
                cmd.Parameters.AddWithValue("@Surname", surname);
                cmd.Parameters.AddWithValue("@FullName", names);
                cmd.Parameters.AddWithValue("@selector", "getVoterbymultiple");
                AddMissingGetAllVotersParameters(cmd);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new AllVoters();
                    v.Province = dr["Province"].ToString();
                    v.Municipality = dr["Municipality"].ToString();
                    v.WardID = dr["WardID"].ToString();
                    v.VDNumber = dr["VDNumber"].ToString();
                    v.VDName = dr["VDName"].ToString();
                    v.IDNumber = dr["IDNumber"].ToString();
                    v.Surname = dr["Surname"].ToString();
                    v.FullName = dr["FullName"].ToString();
                    v.VR_Address = dr["VR_Address"].ToString();
                    v.StreetNumber = dr["StreetNumber"].ToString();
                    v.StreetName = dr["StreetName"].ToString();
                    v.Suburb = dr["Suburb"].ToString();
                    v.Town = dr["Town"].ToString();
                    v.ResPostalCode = dr["ResPostalCode"].ToString();
                    v.VoterID = dr["VoterID"].ToString();
                    v.ID = dr["ID"].ToString();
                    v.Recorded = dr["Recorded"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }

    private static void AddMissingGetAllVotersParameters(MySqlCommand cmd)
    {
        string[] parameterNames =
        {
            "@IDNumber",
            "@WardID",
            "@VDNumber",
            "@Suburb",
            "@Municipality",
            "@Surname",
            "@FullName",
            "@selector"
        };

        Dictionary<string, object?> values = new Dictionary<string, object?>();

        foreach (var parameterName in parameterNames)
        {
            values[parameterName] = cmd.Parameters.Contains(parameterName)
                ? cmd.Parameters[parameterName].Value
                : DBNull.Value;
        }

        cmd.Parameters.Clear();

        foreach (var parameterName in parameterNames)
        {
            cmd.Parameters.AddWithValue(parameterName, values[parameterName] ?? DBNull.Value);
        }
    }
}
