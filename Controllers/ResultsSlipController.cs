using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using Web_Api.Models;
using System.Collections;

namespace Web_Api.Controllers;

[Route("[controller]")]
[ApiController]
public class ResultsSlipController : ControllerBase
{
    private IConfiguration? _config;
    private ResultsSlip? r;
    private string? connect;
    private MySqlDataAdapter? da;
    private DataSet? ds;

    public ResultsSlipController(IConfiguration configuration)
    {
        _config = configuration;
        connect = _config.GetConnectionString("ConsString");
        r = new ResultsSlip();
    }
    //GET ALL
    [HttpGet]
    [Route("getall")]
    public IEnumerable<ResultsSlip> getall()
    {
        MySqlDataReader dr;
        List<ResultsSlip> all = new List<ResultsSlip>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("SELECT Election, Election_date, VD, Municipality, Ward, Candidate_name, Party_name, Votes, Party_agent, Day FROM ResultsSlip", con))
            {
                cmd.CommandType = CommandType.Text;
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    r = new ResultsSlip();
                    //Election,Election_date,VD,Municipality,Ward,Candidate_name,Party_name,Votes,Party_agent
                    r.Election = dr["Election"].ToString();
                    r.Election_date = dr["Election_date"].ToString();
                    r.VD = dr["VD"].ToString();
                    r.Municipality = dr["Municipality"].ToString();
                    r.Ward = dr["Ward"].ToString();
                    r.Candidate_name = dr["Candidate_name"].ToString();
                    r.Party_name = dr["Party_name"].ToString();
                    r.Votes = dr["Votes"].ToString();
                    r.Party_agent = dr["Party_agent"].ToString();
                     r.Day = dr["Day"].ToString();
                    //
                    all.Add(r);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    //GET ALL BY ELECTION DAY
    [HttpGet]
    [Route("getbyday/{day}/{vd}")]
    public IEnumerable<ResultsSlip> getbyday(string day,string vd)
    {
        MySqlDataReader dr;
        List<ResultsSlip> all = new List<ResultsSlip>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("SELECT Election, Election_date, VD, Municipality, Ward, Candidate_name, Party_name, Votes, Party_agent, Day FROM ResultsSlip WHERE Day = @Day AND VD = @VD", con))
            {
                cmd.CommandType = CommandType.Text; 
                cmd.Parameters.AddWithValue("@Day",day);
                 cmd.Parameters.AddWithValue("@VD",vd);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    r = new ResultsSlip();
                   //Election,Election_date,VD,Municipality,Ward,Candidate_name,Party_name,Votes,Party_agent
                    r.Election = dr["Election"].ToString();
                    r.Election_date = dr["Election_date"].ToString();
                    r.VD = dr["VD"].ToString();
                    r.Municipality = dr["Municipality"].ToString();
                    r.Ward = dr["Ward"].ToString();
                    r.Candidate_name = dr["Candidate_name"].ToString();
                    r.Party_name = dr["Party_name"].ToString();
                    r.Votes = dr["Votes"].ToString();
                    r.Party_agent = dr["Party_agent"].ToString();
                    r.Day = dr["Day"].ToString();
                    all.Add(r);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    //GET BY ELECTION
    [HttpGet]
    [Route("getbyelection/{election}")]
    public IEnumerable<ResultsSlip> getbyelection(string election)
    {
        MySqlDataReader dr;
        List<ResultsSlip> all = new List<ResultsSlip>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("SELECT Election, Election_date, VD, Municipality, Ward, Candidate_name, Party_name, Votes, Party_agent, Day FROM ResultsSlip WHERE Election = @Election", con))
            {
                cmd.CommandType = CommandType.Text;
                 cmd.Parameters.AddWithValue("@Election", election);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    r = new ResultsSlip();
                    //Election,Election_date,VD,Municipality,Ward,Candidate_name,Party_name,Votes,Party_agent
                    r.Election = dr["Election"].ToString();
                    r.Election_date = dr["Election_date"].ToString();
                    r.VD = dr["VD"].ToString();
                    r.Municipality = dr["Municipality"].ToString();
                    r.Ward = dr["Ward"].ToString();
                    r.Candidate_name = dr["Candidate_name"].ToString();
                    r.Party_name = dr["Party_name"].ToString();
                    r.Votes = dr["Votes"].ToString();
                    r.Party_agent = dr["Party_agent"].ToString();
                     r.Day = dr["Day"].ToString();
                    //
                    all.Add(r);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    //GET DAY
    [HttpGet]
    [Route("getday/{vd}")]
    public IEnumerable<ResultsSlip> getday(string vd)
    {
        MySqlDataReader dr;
        List<ResultsSlip> all = new List<ResultsSlip>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT Day, Election, VD FROM ResultsSlip WHERE VD = @VD", con))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@VD", vd);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    r = new ResultsSlip();
                    //Election,Election_date,VD,Municipality,Ward,Candidate_name,Party_name,Votes,Party_agent
                    r.Day = dr["Day"].ToString();
                    r.Election = dr["Election"].ToString();
                    r.VD=dr["VD"].ToString();
                    //
                    all.Add(r);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
     //GET VDs
    [HttpGet]
    [Route("getvds/{election}")]
    public IEnumerable<ResultsSlip> getvds(string election)
    {
        MySqlDataReader dr;
        List<ResultsSlip> all = new List<ResultsSlip>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT VD FROM ResultsSlip WHERE Election = @Election", con))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Election", election);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    r = new ResultsSlip();
                    //Election,Election_date,VD,Municipality,Ward,Candidate_name,Party_name,Votes,Party_agent
                    r.VD = dr["VD"].ToString();
                    //
                    all.Add(r);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
     [HttpPost]
    public int addSlip(ResultsSlip slip)
    {
        var retCode = 0;
        // Add the new product to the database
        // ...
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(@"
                INSERT INTO ResultsSlip
                    (Election, Election_date, VD, Municipality, Ward, Candidate_name, Party_name, Votes, Party_agent, Day)
                VALUES
                    (@Election, @Election_date, @VD, @Municipality, @Ward, @Candidate_name, @Party_name, @Votes, @Party_agent, @Day)", con))
            {
                //Election,Election_date,VD,Municipality,Ward,Candidate_name,Party_name,Votes,Party_agent
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Election", slip.Election);
                cmd.Parameters.AddWithValue("@Election_date", slip.Election_date);
                cmd.Parameters.AddWithValue("@VD", slip.VD);
                cmd.Parameters.AddWithValue("@Municipality", slip.Municipality);
                cmd.Parameters.AddWithValue("@Ward", slip.Ward);
                cmd.Parameters.AddWithValue("@Candidate_name", slip.Candidate_name);
                cmd.Parameters.AddWithValue("@Party_name", slip.Party_name);
                cmd.Parameters.AddWithValue("@Votes", slip.Votes);
                cmd.Parameters.AddWithValue("@Party_agent", slip.Party_agent);
                cmd.Parameters.AddWithValue("@Day", slip.Day);
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
