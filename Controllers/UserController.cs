using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text.RegularExpressions;
using Web_Api.Models;

namespace Web_Api.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private IConfiguration? _config;
    private User? u;
    private string? connect;
    

    public UserController(IConfiguration configuration)
    {
        _config = configuration;
        connect = _config.GetConnectionString("ConsString");
        u = new User();
    }
    //GET ALL USERS
    // [HttpGet]
    // public IEnumerable<User> getall()
    // {
    //     MySqlDataReader dr;
    //     List<User> all = new List<User>();
    //     //setup database cons

    //     //initiate connection
    //     using (MySqlConnection con = new MySqlConnection(connect))
    //     {
    //         con.Open();
    //         using (MySqlCommand cmd = new MySqlCommand("getUsers", con))
    //         {
    //             cmd.CommandType = CommandType.StoredProcedure;
    //             cmd.Parameters.AddWithValue("@selector", "all");
    //             //open reader
    //             dr = cmd.ExecuteReader();
    //             //fill data in datatable
    //             while (dr.Read())
    //             {
    //                 u = new User();
    //                 u.Id = dr["number"].ToString();
    //                 u.Name = dr["Name"].ToString();
    //                 u.Surname = dr["Surname"].ToString();
    //                 u.Password = dr["Password"].ToString();
    //                 u.Cell = dr["Cell"].ToString();
    //                 u.Ward = dr["Ward"].ToString();
    //                 u.Voting_Station = dr["Voting_Station"].ToString();
    //                 u.Voting_district = dr["Voting_district"].ToString();
    //                 u.Date = dr["Date"].ToString();
    //                 u.Role = dr["role"].ToString();
    //                 //addion
    //                 u.Delegation = dr["Delegation"].ToString();
    //                 u.Province = dr["Province"].ToString();
    //                 u.Region = dr["Region"].ToString();
    //                 u.Report = dr["Report"].ToString();
    //                 //
    //                 all.Add(u);
    //             }
    //             //close connections
    //             dr.Close();
    //             con.Close();
    //         }
    //     }
    //     return all.ToArray();
    // }
    //GET USERS BY CELL
    [HttpGet]
    [Route("getbycell/{cell}")]
    public IEnumerable<User> getbycell(string cell)
    {
        MySqlDataReader dr;
        List<User> all = new List<User>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getUsers", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Cell", cell);
                cmd.Parameters.AddWithValue("@selector", "bycell");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    u = new User();
                    u.Id = dr["number"].ToString();
                    u.Name = dr["Name"].ToString();
                    u.Surname = dr["Surname"].ToString();
                    u.Password = dr["Password"].ToString();
                    u.Cell = dr["Cell"].ToString();
                    u.Ward = dr["Ward"].ToString();
                    u.Voting_Station = dr["Voting_Station"].ToString();
                    u.Voting_district = dr["Voting_district"].ToString();
                    u.Date = dr["Date"].ToString();
                    u.Role = dr["role"].ToString();
                    //add on
                    u.Delegation = dr["Delegation"].ToString();
                    u.Province = dr["Province"].ToString();
                    u.Region = dr["Region"].ToString();
                    u.Report = dr["Report"].ToString();
                    u.Municipality = dr["Municipality"].ToString();
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
    //GET USERS BY REGION
    [HttpGet]
    [Route("getbyregion/{region}")]
    public IEnumerable<User> getbyregion(string region)
    {
        MySqlDataReader dr;
        List<User> all = new List<User>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getUsers", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Region", region);
                cmd.Parameters.AddWithValue("@selector", "byregion");

                cmd.AddMissingStoredProcedureParameters();

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    u = new User();
                    u.Id = dr["number"].ToString();
                    u.Name = dr["Name"].ToString();
                    u.Surname = dr["Surname"].ToString();
                    u.Password = dr["Password"].ToString();
                    u.Cell = dr["Cell"].ToString();
                    u.Ward = dr["Ward"].ToString();
                    u.Voting_Station = dr["Voting_Station"].ToString();
                    u.Voting_district = dr["Voting_district"].ToString();
                    u.Date = dr["Date"].ToString();
                    u.Role = dr["role"].ToString();
                    u.Delegation = dr["Delegation"].ToString();
                    u.Province = dr["Province"].ToString();
                    u.Region = dr["Region"].ToString();
                    u.Report = dr["Report"].ToString();
                    u.Municipality = dr["Municipality"].ToString();

                    all.Add(u);
                }

                dr.Close();
                con.Close();
            }
        }

        return all.ToArray();
    }
    //GET USERS BY VD
    [HttpGet]
    [Route("getbyvd/{vd}")]
    public IEnumerable<User> getbyvd(int vd)
    {
        MySqlDataReader dr;
        List<User> all = new List<User>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getUsers", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Voting_District", vd);
                cmd.Parameters.AddWithValue("@selector", "byvd");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    u = new User();
                    u.Id = dr["number"].ToString();
                    u.Name = dr["Name"].ToString();
                    u.Surname = dr["Surname"].ToString();
                    u.Password = dr["Password"].ToString();
                    u.Cell = dr["Cell"].ToString();
                    u.Ward = dr["Ward"].ToString();
                    u.Voting_Station = dr["Voting_Station"].ToString();
                    u.Voting_district = dr["Voting_district"].ToString();
                    u.Date = dr["Date"].ToString();
                    u.Role = dr["role"].ToString();
                    //addion
                    u.Delegation = dr["Delegation"].ToString();
                    u.Province = dr["Province"].ToString();
                    u.Region = dr["Region"].ToString();
                    u.Report = dr["Report"].ToString();
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
    //GET USERS BY WARD
    [HttpGet]
    [Route("getbyward/{ward}")]
    public IEnumerable<User> getbyward(string ward)
    {
        MySqlDataReader dr;
        List<User> all = new List<User>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getUsers", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ward", ward);
                cmd.Parameters.AddWithValue("@selector", "byward");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    u = new User();
                    u.Id = dr["number"].ToString();
                    u.Name = dr["Name"].ToString();
                    u.Surname = dr["Surname"].ToString();
                    u.Password = dr["Password"].ToString();
                    u.Cell = dr["Cell"].ToString();
                    u.Ward = dr["Ward"].ToString();
                    u.Voting_Station = dr["Voting_Station"].ToString();
                    u.Voting_district = dr["Voting_district"].ToString();
                    u.Date = dr["Date"].ToString();
                    u.Role = dr["role"].ToString();
                    //addion
                    u.Delegation = dr["Delegation"].ToString();
                    u.Province = dr["Province"].ToString();
                    u.Region = dr["Region"].ToString();
                    u.Report = dr["Report"].ToString();
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
    //GET USERS BY LET MUNICIPALITY
    [HttpGet]
    [Route("getbylet/{municipality}")]
    public IEnumerable<User> getbylet(string municipality)
    {
        MySqlDataReader dr;
        List<User> all = new List<User>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getUsers", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Municipality", municipality);
                cmd.Parameters.AddWithValue("@selector", "getbylet");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    u = new User();
                    u.Id = dr["number"].ToString();
                    u.Name = dr["Name"].ToString();
                    u.Surname = dr["Surname"].ToString();
                    u.Password = dr["Password"].ToString();
                    u.Cell = dr["Cell"].ToString();
                    u.Ward = dr["Ward"].ToString();
                    u.Voting_Station = dr["Voting_Station"].ToString();
                    u.Voting_district = dr["Voting_district"].ToString();
                    u.Date = dr["Date"].ToString();
                    u.Role = dr["role"].ToString();
                    //addion
                    u.Delegation = dr["Delegation"].ToString();
                    u.Province = dr["Province"].ToString();
                    u.Region = dr["Region"].ToString();
                    u.Report = dr["Report"].ToString();
                    u.Municipality = dr["Municipality"].ToString();
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
    //SEARCH USERS BY CELL (phase 2 chat — start a conversation with a non-VD-member).
    //[Authorize] here specifically, even though this controller isn't gated overall — this is
    //a brand new endpoint being added now, not one of the ~80 pre-existing ones, and it exposes
    //PII (name/role/location) by phone-number search so it gets the new-endpoint auth bar.
    [HttpGet]
    [Authorize]
    [Route("searchbycell/{cell}")]
    public IEnumerable<User> SearchByCell(string cell)
    {
        var digits = Regex.Replace(cell ?? "", @"\D", "");
        var all = new List<User>();
        if (digits.Length < 4) // avoid a near-unfiltered scan on a 1-3 digit query
        {
            return all;
        }

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(@"
                SELECT number, Name, Surname, Cell, role, Ward, Voting_District, Municipality
                FROM Users
                WHERE REPLACE(REPLACE(REPLACE(Cell,' ',''),'-',''),'+','') LIKE CONCAT('%', @digits, '%')
                LIMIT 20", con))
            {
                cmd.Parameters.AddWithValue("@digits", digits);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var u = new User();
                        u.Id = dr["number"].ToString();
                        u.Name = dr["Name"].ToString();
                        u.Surname = dr["Surname"].ToString();
                        u.Cell = dr["Cell"].ToString();
                        u.Role = dr["role"].ToString();
                        u.Ward = dr["Ward"].ToString();
                        u.Voting_district = dr["Voting_District"].ToString();
                        u.Municipality = dr["Municipality"].ToString();
                        all.Add(u);
                    }
                }
            }
        }
        return all;
    }

    //GET USERS BY DELEGATION
    [HttpGet]
    [Route("bydelegation/{delegation}")]
    public IEnumerable<User> bydelegation(string delegation)
    {
        MySqlDataReader dr;
        List<User> all = new List<User>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getUsers", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Delegation", delegation);
                cmd.Parameters.AddWithValue("@selector", "bydelegation");
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                while (dr.Read())
                {
                    u = new User();
                    u.Id = dr["number"].ToString();
                    u.Name = dr["Name"].ToString();
                    u.Surname = dr["Surname"].ToString();
                    u.Password = dr["Password"].ToString();
                    u.Cell = dr["Cell"].ToString();
                    u.Ward = dr["Ward"].ToString();
                    u.Voting_Station = dr["Voting_Station"].ToString();
                    u.Voting_district = dr["Voting_district"].ToString();
                    u.Date = dr["Date"].ToString();
                    u.Role = dr["role"].ToString();
                    //addion
                    u.Delegation = dr["Delegation"].ToString();
                    u.Province = dr["Province"].ToString();
                    u.Region = dr["Region"].ToString();
                    u.Report = dr["Report"].ToString();
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
    [HttpGet]
    [Route("getbycellandpass/{cell}/{password}")]
    public User getbycellandpass(string? cell,string password){
      MySqlDataReader dr;
      User user=new User();
      using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getUsers", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Cell", cell);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@selector", "bycellandpass");
                
                //open reader
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
                if (dr.Read())
                {
                   user=new User();
                   user.Name = dr["Name"].ToString();
                   user.Surname = dr["Surname"].ToString();
                   user.Cell = dr["Cell"].ToString();
                   user.Ward = dr["Ward"].ToString();
                   user.Voting_district = dr["Voting_District"].ToString();
                   user.Role=dr["role"].ToString();
                   user.Delegation = dr["Delegation"].ToString();
                   user.Region = dr["Region"].ToString();
                   user.Province = dr["Province"].ToString();
                   user.Municipality = dr["Municipality"].ToString();
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
      return user;
    }
   [HttpPost]
   public async Task<int> AddUser(User user)
    {
        var retCode = 0;
        // Add the new product to the database
        // ...
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getUsers", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Surname", user.Surname);
                cmd.Parameters.AddWithValue("@Cell", user.Cell);
                cmd.Parameters.AddWithValue("@Ward", user.Ward);
                cmd.Parameters.AddWithValue("@Voting_Station", user.Voting_Station);
                cmd.Parameters.AddWithValue("@Voting_District", user.Voting_district);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                //addition
                cmd.Parameters.AddWithValue("@Delegation", user.Delegation);
                cmd.Parameters.AddWithValue("@Province", user.Province);
                cmd.Parameters.AddWithValue("@Region", user.Region);
                cmd.Parameters.AddWithValue("@Report", user.Report);
                cmd.Parameters.AddWithValue("@Municipality", user.Municipality);
                //addition
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@selector", "add");
                //
                //con.Open();
                cmd.AddMissingStoredProcedureParameters();
                retCode = await cmd.ExecuteNonQueryAsync();
                con.Close();
            }
        }
        //return codes

        //  -1 =failue
        //   1 = account saved successfully
        return retCode;
    }
   [HttpPut]
    public async Task<int> updatePassword(User user){
        var retCode = 0;
        // Add the new product to the database
        // ...
         //Console.WriteLine(user);
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getUsers", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Cell", user.Cell);
               
                cmd.Parameters.AddWithValue("@selector", "updatepassword");
                //
                //con.Open();
                cmd.AddMissingStoredProcedureParameters();
                retCode = await cmd.ExecuteNonQueryAsync();
                con.Close();
            }
        }
        //return codes

        //  -1 =failue
        //   1 = account saved successfully
        return retCode;

    }
    [HttpPut]
    [Route("UpdateUser")]  
    public async Task<int> UpdateUser(User user){
        var retCode = 0;
        // Add the new product to the database
        // ...
       
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getUsers", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", Convert.ToInt32(user.Id));
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Surname", user.Surname);
                cmd.Parameters.AddWithValue("@Cell", user.Cell);
                cmd.Parameters.AddWithValue("@Ward", user.Ward);
                cmd.Parameters.AddWithValue("@Voting_Station", user.Voting_Station);
                cmd.Parameters.AddWithValue("@Voting_District", user.Voting_district);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                //addition
                cmd.Parameters.AddWithValue("@Delegation", user.Delegation);
                cmd.Parameters.AddWithValue("@Province", user.Province);
                cmd.Parameters.AddWithValue("@Region", user.Region);
                cmd.Parameters.AddWithValue("@Municipality", user.Municipality);
                //addition
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Report", user.Report);
                cmd.Parameters.AddWithValue("@selector", "updateuser");
                //
                //con.Open();
                //cmd.AddMissingStoredProcedureParameters();
                retCode = await cmd.ExecuteNonQueryAsync();
                con.Close();
            }
        }
        //return codes

        //  -1 =failue
        //   1 = account saved successfully
        return retCode;

    }
    [HttpDelete]
    [Route("DeleteUser/{id}")]
    public async Task<int> DeleteUser(int id)
    {
        var retCode = 0;

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getUsers", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@selector", "deleteuser");

                cmd.AddMissingStoredProcedureParameters();
                retCode = await cmd.ExecuteNonQueryAsync();
                con.Close();
            }
        }

        return retCode;
    }
}
