using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using Web_Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Web_Api.Controllers;

[Route("[controller]")]
[ApiController]
public class RallyController : ControllerBase
{
    private IConfiguration? _config;
    private AllVoters? v;
    private string? connect;

    public RallyController(IConfiguration configuration)
    {
        _config = configuration;
        connect = _config.GetConnectionString("ConsString");
        v = new AllVoters();
    }
    // [HttpGet]
    // [Route("getwardprogress/{ward}")]
    // public IEnumerable<AllVoters> getwardprogress(string ward)
    // {
    //     List<AllVoters> all = new List<AllVoters>();

    //     using (MySqlConnection con = new MySqlConnection(connect))
    //     {
    //         con.Open();
    //         using (MySqlCommand cmd = new MySqlCommand("cgetrally", con))
    //         {
    //             cmd.CommandType = CommandType.StoredProcedure;
    //             cmd.Parameters.AddWithValue("@ward", ward);
    //             cmd.Parameters.AddWithValue("@selector", "wardprogress");

    //             using (MySqlDataReader dr = cmd.ExecuteReader())
    //             {
    //                 while (dr.Read())
    //                 {
    //                     v = new RecordedVoters();
    //                     v.surname = dr["recorded"].ToString();//takes recorded
    //                     v.names = dr["total"].ToString();//takes total
    //                     v.id_number = dr["users"].ToString();//takes users

    //                     all.Add(v);
    //                 }
    //                 //close connections
    //                 dr.Close();
    //                 con.Close();
    //             }
    //         }
    //     }
    //     return all.ToArray();
    // }
    [HttpGet]
    [Route("getprovinceprogress")]
    public IEnumerable<AllVoters> getprovinceprogress()
    {
        List<AllVoters> all = new List<AllVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getrally", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@selector", "getprovinceprogress");

                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new AllVoters();
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
    [HttpGet]
    [Route("getmunicipalityprogress")]
    public IEnumerable<AllVoters> getmunicipalityprogress()
    {
        List<AllVoters> all = new List<AllVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getrally", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@selector", "getmunicipalityprogress");

                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new AllVoters();
                        v.Municipality = dr["Municipality"].ToString();//takes municipality
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
    [Route("getwardlistprogress/{municipality}")]
    public IEnumerable<AllVoters> getwardlistprogress(string municipality)
    {
        List<AllVoters> all = new List<AllVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getrally", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Municipality", municipality);
                cmd.Parameters.AddWithValue("@selector", "getwardlistprogress");

                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new AllVoters();
                        v.WardID = dr["WardID"].ToString();//takes municipality
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
    [Route("vdnumbers/{ward}")]
    public IEnumerable<AllVoters> vdnumbers(string ward)
    {
        List<AllVoters> all = new List<AllVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getrally", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WardID", ward);
                cmd.Parameters.AddWithValue("@selector", "vdnumbers");

                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new AllVoters();
                        v.VDNumber = dr["VDNumber"].ToString();//vd number
                        v.VDName = dr["VDName"].ToString();//vd name
                        v.ID= dr["Total"].ToString();//takes total recorded per vd
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
    public int add(AllVoters voter)
    {
        int retCode = 0;

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            using (MySqlCommand cmd = new MySqlCommand("getrally", con))
            {
                //Province,Municipality,WardID,VDNumber,VRSection,SeqNo,IDNumber,Surname,FullName,StreetNumber,
                //StreetName,StreetType,PlaceNumber,PlaceName,HouseNumber,
                //FarmName,FarmNumber,Location,Suburb,Town,ResPostalCode,Recorded,VDName,VR_Address,VoterID

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@selector", "add");
                //
                cmd.Parameters.AddWithValue("@Province", voter.Province);
                cmd.Parameters.AddWithValue("@Municipality", voter.Municipality);
                cmd.Parameters.AddWithValue("@WardID", voter.WardID);
                cmd.Parameters.AddWithValue("@VDNumber", voter.VDNumber);
                cmd.Parameters.AddWithValue("@VRSection", voter.VRSection);
                cmd.Parameters.AddWithValue("@SeqNo", voter.SeqNo);
                cmd.Parameters.AddWithValue("@IDNumber", voter.IDNumber);
                cmd.Parameters.AddWithValue("@Surname", voter.Surname);
                cmd.Parameters.AddWithValue("@FullName", voter.FullName);

                
                cmd.Parameters.AddWithValue("@StreetNumber", voter.StreetNumber);
                cmd.Parameters.AddWithValue("@StreetName", voter.StreetName);
                cmd.Parameters.AddWithValue("@StreetType", voter.StreetType);
                cmd.Parameters.AddWithValue("@PlaceNumber", voter.PlaceNumber);
                cmd.Parameters.AddWithValue("@PlaceName", voter.PlaceName);

                cmd.Parameters.AddWithValue("@HouseNumber", voter.HouseNumber);
                cmd.Parameters.AddWithValue("@FarmName", voter.FarmName);
                cmd.Parameters.AddWithValue("@FarmNumber", voter.FarmNumber);
                cmd.Parameters.AddWithValue("@Location", voter.Location);
                cmd.Parameters.AddWithValue("@Suburb", voter.Suburb);
                cmd.Parameters.AddWithValue("@Town", voter.Town);
                cmd.Parameters.AddWithValue("@ResPostalCode", voter.ResPostalCode);
                cmd.Parameters.AddWithValue("@ScanType", voter.ScanType);
                cmd.Parameters.AddWithValue("@VDName", voter.VDName);
                cmd.Parameters.AddWithValue("@VR_Address", voter.VR_Address);
                cmd.Parameters.AddWithValue("@VoterID", voter.VoterID);
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
}