using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using Web_Api.Models;

namespace Web_Api.Controllers;

[Route("[controller]")]
[ApiController]
public class RecordedVoters2024Controller : ControllerBase
{
    private IConfiguration _config;
    private RecordedVoters? v;
    private string? connect;

    public RecordedVoters2024Controller(IConfiguration configuration)
    {
        _config = configuration;
        connect = _config.GetConnectionString("ConsString");
        v = new RecordedVoters();
    }
    //GET ALL VOTERS
    [HttpGet]
    public IEnumerable<RecordedVoters> getall()
    {
        MySqlDataReader dr;
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_selector", "all");

                AddMissingGetRecordedVoters2024Parameters(cmd);
                cmd.AddMissingStoredProcedureParameters();

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    v = new RecordedVoters();
                    v.id = dr["id"].ToString();
                    v.surname = dr["surname"].ToString();
                    v.names = dr["names"].ToString();
                    v.id_number = dr["id_number"].ToString();
                    v.address = dr["address"].ToString();
                    v.province = dr["province"].ToString();
                    v.municipality = dr["municipality"].ToString();
                    v.voting_station = dr["voting_station"].ToString();
                    v.ward = dr["ward"].ToString();
                    v.voting_DISTRICT = dr["voting_district"].ToString();
                    v.issuesArr = dr["issuesArr"].ToString();
                    v.status = dr["status"].ToString();
                    v.date = dr["date"].ToString();
                    v.cell = dr["cell"].ToString();
                    v.attitude = dr["attitude"].ToString();
                    v.volunteer = dr["volunteer"].ToString();
                    v.transport = dr["transport"].ToString();
                    v.special = dr["special"].ToString();
                    all.Add(v);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }
    //GET ALL VOTERS BY CELL
    [HttpGet]
    [Route("getbycell/{cell}")]
    public IEnumerable<RecordedVoters> getbycell(string cell)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_cell", cell);
                cmd.Parameters.AddWithValue("@p_selector", "getbycell");

                AddMissingGetRecordedVoters2024Parameters(cmd);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.surname = dr["surname"].ToString();
                        v.names = dr["names"].ToString();
                        v.id_number = dr["id_number"].ToString();
                        v.address = dr["address"].ToString();
                        v.province = dr["province"].ToString();
                        v.municipality = dr["municipality"].ToString();
                        v.voting_station = dr["voting_station"].ToString();
                        v.ward = dr["ward"].ToString();
                        v.voting_DISTRICT = dr["voting_district"].ToString();
                        v.issuesArr = dr["issuesArr"].ToString();
                        v.status = dr["status"].ToString();
                        v.date = dr["date"].ToString();
                        v.cell = dr["cell"].ToString();
                        v.attitude = dr["attitude"].ToString();
                        v.volunteer = dr["volunteer"].ToString();
                        v.transport = dr["transport"].ToString();
                        v.special = dr["special"].ToString();
                        v.election = dr["election"].ToString();
                        all.Add(v);
                    }

                    dr.Close();
                    con.Close();
                }
            }
        }

        return all.ToArray();
    }
    //GET ALL VOTERS BY ID NUMBER AND SURNAME
    [HttpGet]
    [Route("getbyidandsurname/{id}/{surname}")]
    public IEnumerable<RecordedVoters> getbyidandsurname(string id, string surname)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_id_number", id);
                cmd.Parameters.AddWithValue("@p_surname", surname);
                cmd.Parameters.AddWithValue("@p_selector", "getbyidandsurname");

                AddMissingGetRecordedVoters2024Parameters(cmd);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.surname = dr["surname"].ToString();
                        v.names = dr["names"].ToString();
                        v.id_number = dr["id_number"].ToString();
                        v.address = dr["address"].ToString();
                        v.province = dr["province"].ToString();
                        v.municipality = dr["municipality"].ToString();
                        v.voting_station = dr["voting_station"].ToString();
                        v.ward = dr["ward"].ToString();
                        v.voting_DISTRICT = dr["voting_district"].ToString();
                        v.issuesArr = dr["issuesArr"].ToString();
                        v.status = dr["status"].ToString();
                        v.date = dr["date"].ToString();
                        v.cell = dr["cell"].ToString();
                        v.attitude = dr["attitude"].ToString();
                        v.volunteer = dr["volunteer"].ToString();
                        v.transport = dr["transport"].ToString();
                        v.special = dr["special"].ToString();
                        v.election = dr["election"].ToString();
                        all.Add(v);
                    }

                    dr.Close();
                    con.Close();
                }
            }
        }

        return all.ToArray();
    }
    [HttpGet]
    [Route("getbyvolunteer/{cell}")]
    public IEnumerable<RecordedVoters> getbyvolunteer(string cell)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_volunteer", cell);
                cmd.Parameters.AddWithValue("@p_selector", "getbyvolunteer");

                AddMissingGetRecordedVoters2024Parameters(cmd);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.surname = dr["surname"].ToString();
                        v.names = dr["names"].ToString();
                        v.id_number = dr["id_number"].ToString();
                        v.address = dr["address"].ToString();
                        v.province = dr["province"].ToString();
                        v.municipality = dr["municipality"].ToString();
                        v.voting_station = dr["voting_station"].ToString();
                        v.ward = dr["ward"].ToString();
                        v.voting_DISTRICT = dr["voting_district"].ToString();
                        v.status = dr["status"].ToString();
                        v.date = dr["date"].ToString();
                        v.cell = dr["cell"].ToString();
                        v.attitude = dr["attitude"].ToString();
                        v.volunteer = dr["volunteer"].ToString();
                        v.election = dr["election"].ToString();
                        all.Add(v);
                    }

                    dr.Close();
                    con.Close();
                }
            }
        }

        return all.ToArray();
    }
    [HttpGet]
    [Route("getbyward/{ward}")]
    public IEnumerable<RecordedVoters> getbyward(string ward)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_ward", ward);
                cmd.Parameters.AddWithValue("@p_selector", "getbyward");

                AddMissingGetRecordedVoters2024Parameters(cmd);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.surname = dr["surname"].ToString();
                        v.names = dr["names"].ToString();
                        v.id_number = dr["id_number"].ToString();
                        v.address = dr["address"].ToString();
                        v.province = dr["province"].ToString();
                        v.municipality = dr["municipality"].ToString();
                        v.voting_station = dr["voting_station"].ToString();
                        v.ward = dr["ward"].ToString();
                        v.voting_DISTRICT = dr["voting_district"].ToString();
                        v.issuesArr = dr["issuesArr"].ToString();
                        v.status = dr["status"].ToString();
                        v.date = dr["date"].ToString();
                        v.cell = dr["cell"].ToString();
                        v.attitude = dr["attitude"].ToString();
                        v.volunteer = dr["volunteer"].ToString();
                        v.transport = dr["transport"].ToString();
                        v.special = dr["special"].ToString();
                        v.election = dr["election"].ToString();
                        all.Add(v);
                    }

                    dr.Close();
                    con.Close();
                }
            }
        }

        return all.ToArray();
    }
    [HttpGet]
    [Route("getspecial/{vs}")]
    public IEnumerable<RecordedVoters> getspecial(string vs)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_voting_station", vs);
                cmd.Parameters.AddWithValue("@p_selector", "getspecial");

                AddMissingGetRecordedVoters2024Parameters(cmd);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.surname = dr["surname"].ToString();
                        v.names = dr["names"].ToString();
                        v.id_number = dr["id_number"].ToString();
                        v.address = dr["address"].ToString();
                        v.province = dr["province"].ToString();
                        v.municipality = dr["municipality"].ToString();
                        v.voting_station = dr["voting_station"].ToString();
                        v.ward = dr["ward"].ToString();
                        v.voting_DISTRICT = dr["voting_district"].ToString();
                        v.issuesArr = dr["issuesArr"].ToString();
                        v.status = dr["status"].ToString();
                        v.date = dr["date"].ToString();
                        v.cell = dr["cell"].ToString();
                        v.attitude = dr["attitude"].ToString();
                        v.volunteer = dr["volunteer"].ToString();
                        v.transport = dr["transport"].ToString();
                        v.special = dr["special"].ToString();
                        v.election = dr["election"].ToString();
                        all.Add(v);
                    }

                    dr.Close();
                    con.Close();
                }
            }
        }

        return all.ToArray();
    }
    [HttpGet]
    [Route("gettransport/{vs}")]
    public IEnumerable<RecordedVoters> gettransport(string vs)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_voting_station", vs);
                cmd.Parameters.AddWithValue("@p_selector", "gettransport");

                AddMissingGetRecordedVoters2024Parameters(cmd);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.surname = dr["surname"].ToString();
                        v.names = dr["names"].ToString();
                        v.id_number = dr["id_number"].ToString();
                        v.address = dr["address"].ToString();
                        v.province = dr["province"].ToString();
                        v.municipality = dr["municipality"].ToString();
                        v.voting_station = dr["voting_station"].ToString();
                        v.ward = dr["ward"].ToString();
                        v.voting_DISTRICT = dr["voting_district"].ToString();
                        v.issuesArr = dr["issuesArr"].ToString();
                        v.status = dr["status"].ToString();
                        v.date = dr["date"].ToString();
                        v.cell = dr["cell"].ToString();
                        v.attitude = dr["attitude"].ToString();
                        v.volunteer = dr["volunteer"].ToString();
                        v.transport = dr["transport"].ToString();
                        v.special = dr["special"].ToString();
                        v.election = dr["election"].ToString();
                        all.Add(v);
                    }

                    dr.Close();
                    con.Close();
                }
            }
        }

        return all.ToArray();
    }
    [HttpGet]
    [Route("getwardprogress/{ward}/{startdate}/{electiondate}")]
    public IEnumerable<RecordedVoters> getwardprogress(string ward, string startdate, string electiondate)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_ward", ward);
                cmd.Parameters.AddWithValue("@p_start_date", startdate);
                cmd.Parameters.AddWithValue("@p_date", electiondate);
                cmd.Parameters.AddWithValue("@p_selector", "wardprogress");

                AddMissingGetRecordedVoters2024Parameters(cmd);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.surname = dr["recorded"].ToString();//takes recorded
                        v.names = dr["total"].ToString();//takes total
                        v.id_number = dr["users"].ToString();//takes users

                        all.Add(v);
                    }

                    dr.Close();
                    con.Close();
                }
            }
        }

        return all.ToArray();
    }
    [HttpGet]
    [Route("getprovinceprogress/{election}/{startdate}/{date}")]
    public IEnumerable<RecordedVoters> getprovinceprogress(string election, string startdate, string date)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_election", election);
                cmd.Parameters.AddWithValue("@p_start_date", startdate);
                cmd.Parameters.AddWithValue("@p_date", date);
                cmd.Parameters.AddWithValue("@p_selector", "getprovinceprogress");

                AddMissingGetRecordedVoters2024Parameters(cmd);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.surname = dr["recorded"].ToString();//takes recorded
                        v.names = dr["total"].ToString();//takes total
                        v.id_number = dr["users"].ToString();//takes users

                        all.Add(v);
                    }

                    dr.Close();
                    con.Close();
                }
            }
        }

        return all.ToArray();
    }
    [HttpGet]
    [Route("getmunicipalityprogress/{election}/{startdate}/{date}")]
    public IEnumerable<RecordedVoters> getmunicipalityprogress(string election, string startdate, string date)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_election", election);
                cmd.Parameters.AddWithValue("@p_start_date", startdate);
                cmd.Parameters.AddWithValue("@p_date", date);
                cmd.Parameters.AddWithValue("@p_selector", "getmunicipalityprogress");

                AddMissingGetRecordedVoters2024Parameters(cmd);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.municipality = dr["municipality"].ToString();//takes municipality
                        v.id = dr["scanned"].ToString();//takes scanned

                        all.Add(v);
                    }

                    dr.Close();
                    con.Close();
                }
            }
        }

        return all.ToArray();
    }
    [HttpGet]
    [Route("getwardlistprogress/{municipality}")]
    public IEnumerable<RecordedVoters> getwardlistprogress(string municipality)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_municipality", municipality);
                cmd.Parameters.AddWithValue("@p_selector", "getwardlistprogress");

                AddMissingGetRecordedVoters2024Parameters(cmd);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.ward = dr["ward"].ToString();//takes ward
                        v.id = dr["scanned"].ToString();//takes scanned

                        all.Add(v);
                    }

                    dr.Close();
                    con.Close();
                }
            }
        }

        return all.ToArray();
    }
    [HttpGet]
    [Route("countcontact/{id}/{surname}")]
    public IEnumerable<RecordedVoters> countcontact(string id, string surname)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_id_number", id);
                cmd.Parameters.AddWithValue("@p_surname", surname);
                cmd.Parameters.AddWithValue("@p_selector", "countcontact");

                AddMissingGetRecordedVoters2024Parameters(cmd);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.surname = dr["Total"].ToString();//total CONTACT
                        v.volunteer = dr["volunteer"].ToString();//contact volunteer
                        v.date = dr["date"].ToString();//date contacted
                        all.Add(v);
                    }

                    dr.Close();
                    con.Close();
                }
            }
        }

        return all.ToArray();
    }
    [HttpGet]
    [Route("vdnumbers/{ward}/{startdate}/{electiondate}")]
    public IEnumerable<RecordedVoters> vdnumbers(string ward, string startdate, string electiondate)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_ward", ward);
                cmd.Parameters.AddWithValue("@p_start_date", startdate);
                cmd.Parameters.AddWithValue("@p_date", electiondate);
                cmd.Parameters.AddWithValue("@p_selector", "vdnumbers");

                AddMissingGetRecordedVoters2024Parameters(cmd);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.voting_DISTRICT = dr["VDNumber"].ToString();//vd number
                        v.voting_station = dr["VDName"].ToString();//vd name
                        v.id = dr["Total"].ToString();//takes total recorded per vd
                        all.Add(v);
                    }

                    dr.Close();
                    con.Close();
                }
            }
        }

        return all.ToArray();
    }
    [HttpPost]
    public int addRecorded(RecordedVoters vdata)
    {
        int retCode = 0;

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_selector", "add");
                cmd.Parameters.AddWithValue("@p_surname", vdata.surname);
                cmd.Parameters.AddWithValue("@p_names", vdata.names);
                cmd.Parameters.AddWithValue("@p_id_number", vdata.id_number);
                cmd.Parameters.AddWithValue("@p_address", vdata.address);
                cmd.Parameters.AddWithValue("@p_province", vdata.province);
                cmd.Parameters.AddWithValue("@p_municipality", vdata.municipality);
                cmd.Parameters.AddWithValue("@p_voting_station", vdata.voting_station);
                cmd.Parameters.AddWithValue("@p_ward", vdata.ward);
                cmd.Parameters.AddWithValue("@p_voting_district", vdata.voting_DISTRICT);
                cmd.Parameters.AddWithValue("@p_cell", vdata.cell);
                cmd.Parameters.AddWithValue("@p_attitude", vdata.attitude);
                cmd.Parameters.AddWithValue("@p_issuesArr", vdata.issuesArr);
                cmd.Parameters.AddWithValue("@p_special", vdata.special);
                cmd.Parameters.AddWithValue("@p_transport", vdata.transport);
                cmd.Parameters.AddWithValue("@p_volunteer", vdata.volunteer);
                cmd.Parameters.AddWithValue("@p_election", vdata.election);
                cmd.Parameters.AddWithValue("@p_gender", vdata.gender);

                AddMissingGetRecordedVoters2024Parameters(cmd);
                con.Open();
                cmd.AddMissingStoredProcedureParameters();
                var ret = cmd.ExecuteNonQuery();
                retCode = Convert.ToInt32(ret);
                con.Close();
            }
        }
        //  -1 =failue
        //   1 = account saved successfully
        return retCode;
    }
    [HttpGet]
    [Route("gettop10")]
    public IEnumerable<RecordedVoters> gettop10()
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_selector", "gettop10");

                AddMissingGetRecordedVoters2024Parameters(cmd);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.surname = dr["surname"].ToString();
                        v.names = dr["names"].ToString();
                        v.id_number = dr["id_number"].ToString();
                        v.address = dr["address"].ToString();
                        v.province = dr["province"].ToString();
                        v.municipality = dr["municipality"].ToString();
                        v.voting_station = dr["voting_station"].ToString();
                        v.ward = dr["ward"].ToString();
                        v.voting_DISTRICT = dr["voting_district"].ToString();
                        v.issuesArr = dr["issuesArr"].ToString();
                        v.status = dr["status"].ToString();
                        v.date = dr["date"].ToString();
                        v.cell = dr["cell"].ToString();
                        v.attitude = dr["attitude"].ToString();
                        v.volunteer = dr["volunteer"].ToString();
                        v.transport = dr["transport"].ToString();
                        v.special = dr["special"].ToString();
                        v.election = dr["election"].ToString();
                        all.Add(v);
                    }

                    dr.Close();
                    con.Close();
                }
            }
        }

        return all.ToArray();
    }
    [HttpGet]
    [Route("getcanvassedtotal")]
    public IEnumerable<RecordedVoters> getcanvassedtotal()
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_selector", "getcanvassedtotal");

                AddMissingGetRecordedVoters2024Parameters(cmd);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.municipality = dr["municipality"].ToString();
                        v.id = dr["canvassed"].ToString();

                        all.Add(v);
                    }

                    dr.Close();
                    con.Close();
                }
            }
        }

        return all.ToArray();
    }
    [HttpGet]
    [Route("getwardlist")]
    public IEnumerable<RecordedVoters> getwardlist()
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_selector", "getwardlist");

                AddMissingGetRecordedVoters2024Parameters(cmd);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.ward = dr["ward"].ToString();
                        v.municipality = dr["municipality"].ToString();

                        all.Add(v);
                    }

                    dr.Close();
                    con.Close();
                }
            }
        }

        return all.ToArray();
    }
    //get cell number
    [HttpGet]
    [Route("getcellnumber")]
    public IEnumerable<RecordedVoters> getcellnumber()
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters2024", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_selector", "getcellnumbers");

                AddMissingGetRecordedVoters2024Parameters(cmd);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.cell = dr["cell"].ToString();

                        all.Add(v);
                    }

                    dr.Close();
                    con.Close();
                }
            }
        }

        return all.ToArray();
    }

    private static void AddMissingGetRecordedVoters2024Parameters(MySqlCommand cmd)
    {
        string[] parameterNames =
        {
            "@p_surname",
            "@p_names",
            "@p_id_number",
            "@p_address",
            "@p_province",
            "@p_municipality",
            "@p_voting_station",
            "@p_ward",
            "@p_voting_district",
            "@p_cell",
            "@p_attitude",
            "@p_issuesArr",
            "@p_special",
            "@p_transport",
            "@p_volunteer",
            "@p_election",
            "@p_gender",
            "@p_start_date",
            "@p_date",
            "@p_selector"
        };

        Dictionary<string, object?> values = new Dictionary<string, object?>();

        foreach (var parameterName in parameterNames)
        {
            values[parameterName] = GetParameterValue(cmd, parameterName);
        }

        cmd.Parameters.Clear();

        foreach (var parameterName in parameterNames)
        {
            cmd.Parameters.AddWithValue(parameterName, values[parameterName] ?? DBNull.Value);
        }
    }

    private static object GetParameterValue(MySqlCommand cmd, string parameterName)
    {
        string[] candidates =
        {
            parameterName,
            parameterName.TrimStart('@'),
            parameterName.ToLowerInvariant(),
            parameterName.TrimStart('@').ToLowerInvariant()
        };

        foreach (var candidate in candidates)
        {
            if (cmd.Parameters.Contains(candidate))
            {
                return cmd.Parameters[candidate].Value;
            }
        }

        return DBNull.Value;
    }
}
