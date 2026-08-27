using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using Web_Api.Models;

namespace Web_Api.Controllers;

[Route("[controller]")]
[ApiController]
public class FrontDeskController : ControllerBase
{
    private IConfiguration _config;
    private FrontDesk? v;
    private string? connect;

    public FrontDeskController(IConfiguration configuration)
    {
        _config = configuration;
        connect = _config.GetConnectionString("ConsString");
        v = new FrontDesk();
    }
    //GET ALL VOTERS CAPRURED
    [HttpGet]
    [Route("getall")]
    public IEnumerable<FrontDesk> getall()
    {
        MySqlDataReader dr;
        List<FrontDesk> all = new List<FrontDesk>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getFrontDeskRecorded", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Sender", "view");
                AddMissingGetFrontDeskRecordedParameters(cmd);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new FrontDesk();
                    v.Province = dr["Province"].ToString();
                    v.Municipality = dr["Municipality"].ToString();
                    v.WardID = dr["WardID"].ToString();
                    v.VDNumber = dr["VDNumber"].ToString();
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
                    v.Election=dr["Election"].ToString();
                    v.ScanVD=dr["ScanVD"].ToString();
                    v.Date= dr["Date"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
     //GET ALL VOTERS CAPTURED BY WARD
    [HttpGet]
    [Route("getbyward/{ward}")]
    public IEnumerable<FrontDesk> getbyward(string ward)
    {
        MySqlDataReader dr;
        List<FrontDesk> all = new List<FrontDesk>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getFrontDeskRecorded", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("WardID",ward);
                cmd.Parameters.AddWithValue("Sender", "byward");
                AddMissingGetFrontDeskRecordedParameters(cmd);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new FrontDesk();
                    v.Province = dr["Province"].ToString();
                    v.Municipality = dr["Municipality"].ToString();
                    v.WardID = dr["WardID"].ToString();
                    v.VDNumber = dr["VDNumber"].ToString();
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
                    v.Election=dr["Election"].ToString();
                    v.Date= dr["Date"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
     //GET ALL VOTERS CAPTURED BY VD
    [HttpGet]
    [Route("getbyvd/{vd}")]
    public IEnumerable<FrontDesk> getbyvd(string vd)
    {
        MySqlDataReader dr;
        List<FrontDesk> all = new List<FrontDesk>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getFrontDeskRecorded", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("VDNumber",vd);
                cmd.Parameters.AddWithValue("Sender", "byvd");
                AddMissingGetFrontDeskRecordedParameters(cmd);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new FrontDesk();
                    v.Province = dr["Province"].ToString();
                    v.Municipality = dr["Municipality"].ToString();
                    v.WardID = dr["WardID"].ToString();
                    v.VDNumber = dr["VDNumber"].ToString();
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
                    v.Election=dr["Election"].ToString();
                    v.Date= dr["Date"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    //GET ALL VOTERS CAPTURED BY ELECTION
    [HttpGet]
    [Route("getbyelection/{election}")]
    public IEnumerable<FrontDesk> getbyelection(string election)
    {
        MySqlDataReader dr;
        List<FrontDesk> all = new List<FrontDesk>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getFrontDeskRecorded", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Election", election);
                cmd.Parameters.AddWithValue("@Sender", "byelection");
                AddMissingGetFrontDeskRecordedParameters(cmd);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new FrontDesk();
                    v.Province = dr["Province"].ToString();
                    v.Municipality = dr["Municipality"].ToString();
                    v.WardID = dr["WardID"].ToString();
                    v.VDNumber = dr["VDNumber"].ToString();
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
                    v.Election=dr["Election"].ToString();
                    v.Date= dr["Date"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    //GET ALL VOTERS CAPTURED BY ELECTION
    [HttpGet]
    [Route("onewardnumbers/{ward}/{start}/{end}")]
    public IEnumerable<FrontDesk> onewardnumbers(string ward, string start, string end)
    {
        MySqlDataReader dr;
        List<FrontDesk> all = new List<FrontDesk>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getFrontDeskRecorded", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WardID", ward);
                cmd.Parameters.AddWithValue("@start_date", Convert.ToDateTime(start));
                cmd.Parameters.AddWithValue("@end_date", Convert.ToDateTime(end));
                cmd.Parameters.AddWithValue("@Sender", "onewardnumbers");
                AddMissingGetFrontDeskRecordedParameters(cmd);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new FrontDesk();
                    //scanned voters
                    v.Province = dr["scanned"].ToString();
                    //total voters
                    v.Municipality = dr["total"].ToString();
                   
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    //GET ALL VOTERS CAPTURED BY ELECTION
    [HttpGet]
    [Route("vdnumbers/{ward}")]
    public IEnumerable<FrontDesk> vdnumbers(string ward)
    {
        MySqlDataReader dr;
        List<FrontDesk> all = new List<FrontDesk>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getFrontDeskRecorded", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                 cmd.Parameters.AddWithValue("@WardID", ward);
                cmd.Parameters.AddWithValue("@Sender", "vdnumbers");
                AddMissingGetFrontDeskRecordedParameters(cmd);
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    v = new FrontDesk();
                    //VD Number
                    v.VDNumber = dr["VDNumber"].ToString();
                    //total scanned
                    v.Municipality = dr["Total"].ToString();
                   
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
    [Route("getmunicipalityprogress/{election}/{date}")]
    public IEnumerable<FrontDesk> getmunicipalityprogress(string election,string date)
    {
        List<FrontDesk> all = new List<FrontDesk>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getFrontDeskRecorded", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Election", election);
                cmd.Parameters.AddWithValue("@Date", date);
                cmd.Parameters.AddWithValue("@Sender", "getmunicipalityprogress");
                AddMissingGetFrontDeskRecordedParameters(cmd);

                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new FrontDesk();
                        v.Municipality = dr["municipality"].ToString();//takes municipality
                        v.ID = dr["scanned"].ToString();//takes scanned

                        all.Add(v);
                    }
                    //close connections
                    dr.Close();
                    con.Close();
                }
            }
        }
        return all.ToArray();
    }
    [HttpGet]
    [Route("getprovinceprogress/{election}/{date}")]
    public IEnumerable<FrontDesk> getprovinceprogress(string election,string date)
    {
        List<FrontDesk> all = new List<FrontDesk>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getFrontDeskRecorded", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Election", election);
                cmd.Parameters.AddWithValue("@Date", date);
                cmd.Parameters.AddWithValue("@Sender", "getprovinceprogress");
                AddMissingGetFrontDeskRecordedParameters(cmd);

                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new FrontDesk();
                        v.Surname = dr["recorded"].ToString();//takes recorded
                        v.FullName = dr["total"].ToString();//takes total
                        v.IDNumber = dr["users"].ToString();//takes users

                        all.Add(v);
                    }
                    //close connections
                    dr.Close();
                    con.Close();
                }
            }
        }
        return all.ToArray();
    }
    [HttpPost]
    public int add(FrontDesk vdata)
    {
        int retCode = 0;

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            using (MySqlCommand cmd = new MySqlCommand("getFrontDeskRecorded", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Sender", "add");
                //Location,Suburb,Town,ResPostalCode,VoterID,Election
                cmd.Parameters.AddWithValue("@Province", vdata.Province);
                cmd.Parameters.AddWithValue("@Municipality", vdata.Municipality);
                cmd.Parameters.AddWithValue("@WardID", vdata.WardID);
                cmd.Parameters.AddWithValue("@VDNumber", vdata.VDNumber);
                cmd.Parameters.AddWithValue("@VRSection", vdata.VRSection);
                cmd.Parameters.AddWithValue("@SeqNo", vdata.SeqNo);
                cmd.Parameters.AddWithValue("@IDNumber", vdata.IDNumber);
                cmd.Parameters.AddWithValue("@Surname", vdata.Surname);
                cmd.Parameters.AddWithValue("@FullName", vdata.FullName);

                cmd.Parameters.AddWithValue("@VR_Address", vdata.VR_Address);
                cmd.Parameters.AddWithValue("@StreetNumber", vdata.StreetNumber);
                cmd.Parameters.AddWithValue("@StreetName", vdata.StreetName);
                cmd.Parameters.AddWithValue("@StreetType", vdata.StreetType);
                cmd.Parameters.AddWithValue("@PlaceNumber", vdata.PlaceNumber);
                cmd.Parameters.AddWithValue("@PlaceName", vdata.PlaceName);

                cmd.Parameters.AddWithValue("@HouseNumber", vdata.HouseNumber);
                cmd.Parameters.AddWithValue("@FarmName", vdata.FarmName);
                cmd.Parameters.AddWithValue("@FarmNumber", vdata.FarmNumber);
                cmd.Parameters.AddWithValue("@Location", vdata.Location);
                cmd.Parameters.AddWithValue("@Suburb", vdata.Suburb);
                cmd.Parameters.AddWithValue("@Town", vdata.Town);
                cmd.Parameters.AddWithValue("@ResPostalCode", vdata.ResPostalCode);
                cmd.Parameters.AddWithValue("@VoterID", vdata.VoterID);

                cmd.Parameters.AddWithValue("@ScanVD", vdata.ScanVD);
                cmd.Parameters.AddWithValue("@Election", vdata.Election);
                cmd.Parameters.AddWithValue("@ID", vdata.ID);
                AddMissingGetFrontDeskRecordedParameters(cmd);
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
    //FOR NATIONAL AND PROVINCIAL ELECTIONS
    [HttpGet]
    [Route("getprovincialmunicipalities")]
    public IEnumerable<FrontDesk> getprovincialmunicipalities()
    {
        List<FrontDesk> all = new List<FrontDesk>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getFrontDeskRecorded", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Sender", "getprovincialmunicipalities");
                AddMissingGetFrontDeskRecordedParameters(cmd);

                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new FrontDesk();
                        v.Municipality = dr["Municipality"].ToString();//takes recorded
                        v.FullName = dr["scanned"].ToString();//takes total
                        all.Add(v);
                    }
                    //close connections
                    dr.Close();
                    con.Close();
                }
            }
        }
        return all.ToArray();
    }
    [HttpGet]
    [Route("getmunicipalitywards/{municipality}")]
    public IEnumerable<FrontDesk> getmunicipalitywards(string municipality)
    {
        List<FrontDesk> all = new List<FrontDesk>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getFrontDeskRecorded", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Municipality",municipality);
                cmd.Parameters.AddWithValue("@Sender", "getmunicipalitywards");
                AddMissingGetFrontDeskRecordedParameters(cmd);

                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new FrontDesk();
                        v.WardID = dr["WardID"].ToString();//takes recorded
                        v.FullName = dr["scanned"].ToString();//takes total
                        all.Add(v);
                    }
                    //close connections
                    dr.Close();
                    con.Close();
                }
            }
        }
        return all.ToArray();
    }
    [HttpGet]
    [Route("getwardvds/{ward}/{start}/{end}")]
    public IEnumerable<FrontDesk> getwardvds(string ward,string start, string end)
    {
        List<FrontDesk> all = new List<FrontDesk>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getFrontDeskRecorded", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WardID",ward);
                cmd.Parameters.AddWithValue("@start_date",Convert.ToDateTime(start));
                cmd.Parameters.AddWithValue("@end_date",Convert.ToDateTime(end));
                cmd.Parameters.AddWithValue("@Sender", "getwardvds");
                AddMissingGetFrontDeskRecordedParameters(cmd);

                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new FrontDesk();
                        v.VDNumber = dr["VDNumber"].ToString();//takes recorded
                        v.VDName = dr["VDName"].ToString();//takes recorded
                        v.FullName = dr["scanned"].ToString();//takes total
                        v.Surname = dr["total"].ToString();//takes total
                        all.Add(v);
                    }
                    //close connections
                    dr.Close();
                    con.Close();
                }
            }
        }
        return all.ToArray();
    }

    //-------------------------------------------------------
    // GET REGIONAL COUNT
    //-------------------------------------------------------
    [HttpGet]
    [Route("getregionalcount")]
    public IEnumerable<FrontDeskCount> getregionalcount(string startDate, string endDate)
    {
        List<FrontDeskCount> all = new List<FrontDeskCount>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getFrontDeskRecorded", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Sender", "regionalcount");
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var num = new FrontDeskCount();
                        num.region = dr["Region"].ToString();
                        num.total1 = Convert.ToInt32(dr["Total"].ToString());
                        num.total0 = Convert.ToInt32(dr["Total0"].ToString());
                        all.Add(num);
                    }
                    dr.Close();
                    con.Close();
                }
            }
        }
        return all.ToArray();
    }

    //-------------------------------------------------------
    // GET MUNICIPAL COUNT
    //-------------------------------------------------------
    [HttpGet]
    [Route("getmunicipalcount")]
    public IEnumerable<FrontDeskCount> getmunicipalcount(string region, string startDate, string endDate)
    {
        List<FrontDeskCount> all = new List<FrontDeskCount>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getFrontDeskRecorded", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@region", region);
                cmd.Parameters.AddWithValue("@Sender", "municipalcount");
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var num = new FrontDeskCount();
                        num.municipality = dr["Municipality"].ToString();
                        num.total1 = Convert.ToInt32(dr["Total"].ToString());
                        num.total0 = Convert.ToInt32(dr["Total0"].ToString());
                        all.Add(num);
                    }
                    dr.Close();
                    con.Close();
                }
            }
        }
        return all.ToArray();
    }

    //-------------------------------------------------------
    // GET WARD COUNT
    //-------------------------------------------------------
    [HttpGet]
    [Route("getwardcount")]
    public IEnumerable<FrontDeskCount> getwardcount(string municipality, string startDate, string endDate)
    {
        List<FrontDeskCount> all = new List<FrontDeskCount>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getFrontDeskRecorded", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Municipality", municipality);
                cmd.Parameters.AddWithValue("@Sender", "wardcount");
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var num = new FrontDeskCount();
                        num.municipality = dr["WardID"].ToString();
                        num.total1 = Convert.ToInt32(dr["Total"].ToString());
                        num.total0 = Convert.ToInt32(dr["Total0"].ToString());
                        all.Add(num);
                    }
                    dr.Close();
                    con.Close();
                }
            }
        }
        return all.ToArray();
    }

    //-------------------------------------------------------
    // GET VD COUNT
    //-------------------------------------------------------
    [HttpGet]
    [Route("getvdcount")]
    public IEnumerable<FrontDeskCount> getvdcount(string ward, string startDate, string endDate)
    {
        List<FrontDeskCount> all = new List<FrontDeskCount>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getFrontDeskRecorded", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WardID", ward);
                cmd.Parameters.AddWithValue("@Sender", "vdcount");
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);
                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var num = new FrontDeskCount();
                        num.vdNumber = dr["VDNumber"].ToString();
                        num.vdName = dr["VDName"].ToString();
                        num.total1 = Convert.ToInt32(dr["Total"].ToString());
                        num.total0 = Convert.ToInt32(dr["Total0"].ToString());
                        all.Add(num);
                    }
                    dr.Close();
                    con.Close();
                }
            }
        }
        return all.ToArray();
    }

    private static void AddMissingGetFrontDeskRecordedParameters(MySqlCommand cmd)
    {
        string[] parameterNames =
        {
            "@Province",
            "@Municipality",
            "@WardID",
            "@VDNumber",
            "@VRSection",
            "@SeqNo",
            "@IDNumber",
            "@Surname",
            "@FullName",
            "@VR_Address",
            "@StreetNumber",
            "@StreetName",
            "@StreetType",
            "@PlaceNumber",
            "@PlaceName",
            "@HouseNumber",
            "@FarmName",
            "@FarmNumber",
            "@Location",
            "@Suburb",
            "@Town",
            "@ResPostalCode",
            "@VoterID",
            "@Election",
            "@ID",
            "@Date",
            "@ScanVD",
            "@Sender"
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
        if (cmd.Parameters.Contains(parameterName))
        {
            return cmd.Parameters[parameterName].Value;
        }

        string nameWithoutPrefix = parameterName.TrimStart('@');
        if (cmd.Parameters.Contains(nameWithoutPrefix))
        {
            return cmd.Parameters[nameWithoutPrefix].Value;
        }

        return DBNull.Value;
    }
}
