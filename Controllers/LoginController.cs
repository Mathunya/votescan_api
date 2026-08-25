 using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using Web_Api.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Web_Api.Controllers;

[Route("[controller]")]
[ApiController]
public class LoginController : ControllerBase{
    private readonly IConfiguration _config;
    private readonly ILogger<LoginController> _logger;
    private Credentials? a;
    private string? _connect;
    private User user;
    private readonly IHubContext<SessionHub>? _hubContext;

    public LoginController(IConfiguration configuration,IHubContext<SessionHub> hubContext, ILogger<LoginController> logger)
    {
        _config = configuration;
        _logger = logger;
        _connect = _config.GetConnectionString("ConsString");
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        user=new User();
        a = new Credentials();
    }

   
    // ✅ POST (secure)
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Credentials request)
    {
        if (request == null)
            return BadRequest("Request is null");

        if (string.IsNullOrWhiteSpace(request.username) ||
            string.IsNullOrWhiteSpace(request.password)){
            return BadRequest("Username or password missing");
        }
        User? user;
        try
        {
            user = await CheckLoginAsync(request.username, request.password);
        }
        catch (MySqlException ex) when (IsDatabaseUnavailable(ex))
        {
            _logger.LogError(ex, "Unable to connect to the database while checking login.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Database server is unavailable. Check the MariaDB host, port, VPN/network, and firewall.");
        }

        if (user == null || string.IsNullOrEmpty(user.Cell))
            return Unauthorized("Invalid credentials");

        // 🔑 create sessionId (important for SignalR control)
        var sessionId = Guid.NewGuid().ToString();

        try
        {
            await SaveSessionAsync(user.Cell, sessionId);
        }
        catch (MySqlException ex) when (IsDatabaseUnavailable(ex))
        {
            _logger.LogError(ex, "Unable to connect to the database while saving login session.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Database server is unavailable. Check the MariaDB host, port, VPN/network, and firewall.");
        }

        var token = GenerateJwtToken(user, sessionId);

        return Ok(new
        {
            token,
            sessionId,
        });
    }
    [HttpGet]
    [Route("getToken/{cell}/{password}")]
    public  async Task<Token> getToken(string cell,string password)
    {
        Token token=new Token();
         var sessionId = Guid.NewGuid().ToString();

        User? user;
        try
        {
            user = await CheckLoginAsync(cell, password);
        }
        catch (MySqlException ex) when (IsDatabaseUnavailable(ex))
        {
            _logger.LogError(ex, "Unable to connect to the database while generating token.");
            token.token = "Database server is unavailable. Check the MariaDB host, port, VPN/network, and firewall.";
            return token;
        }
        if (user==null)
        {
            token.token= "No user found";
            return token;
        }
        token.token=GenerateJwtToken(user,sessionId);
        return token;
    }
    private async Task<User?> CheckLoginAsync(string cell, string password)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();

        using var cmd = new MySqlCommand("getUsers", con);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Cell", cell);
        cmd.Parameters.AddWithValue("@Password", password);
        cmd.Parameters.AddWithValue("@selector", "getlogin");

        cmd.AddMissingStoredProcedureParameters();

        using var reader = await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new User
        {
            Name = reader["Name"]?.ToString(),
            Surname = reader["Surname"]?.ToString(),
            Cell = reader["Cell"]?.ToString(),
            Ward = reader["Ward"]?.ToString(),
            Voting_district = reader["Voting_District"]?.ToString(),
            Role = reader["role"]?.ToString(),
            Delegation = reader["Delegation"]?.ToString(),
            Region = reader["Region"]?.ToString(),
            Province = reader["Province"]?.ToString(),
            Municipality = reader["Municipality"]?.ToString()
        };
    }
    
    
    private async Task SaveSessionAsync(string userId, string sessionId)
    {
        using var con = new MySqlConnection(_connect);
        await con.OpenAsync();

        var cmd = new MySqlCommand(@"
            INSERT INTO UserSessions (UserId, SessionId, IsActive, LastSeen)
            VALUES (@userId, @sessionId, 1, NOW())
        ", con);

        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@sessionId", sessionId);

        cmd.AddMissingStoredProcedureParameters();

        await cmd.ExecuteNonQueryAsync();
    }

    private static bool IsDatabaseUnavailable(MySqlException ex)
    {
        return ex.InnerException is TimeoutException ||
               ex.Message.Contains("Timeout expired", StringComparison.OrdinalIgnoreCase) ||
               ex.Message.Contains("Unable to connect", StringComparison.OrdinalIgnoreCase) ||
               ex.Message.Contains("server is not responding", StringComparison.OrdinalIgnoreCase);
    }
     
    private string GenerateJwtToken(User user, string sessionId)
    {
        var claims = new List<Claim>
        {
            new Claim("Cell", user.Cell!),
            new Claim("SessionId", sessionId), // 🔥 important
            new Claim("Role", user.Role ?? ""),
            ///
            ///  new Claim("Name",user.Name!),
            new Claim("Surname",user.Surname!),
            new Claim("Delegation",user.Delegation!),
            new Claim("Ward",user.Ward!),
            new Claim("VD",user.Voting_district!),
            new Claim("Region",user.Region!),
            new Claim("Municipality",user.Municipality!),
            new Claim("Province",user.Province!)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
     // [HttpGet]
    // [Route("getToken/{cell}/{password}")]
    // public Token getToken(string cell,string password)
    // {
    //     Token token=new Token();

    //     user=checkLogin(cell,password);
    //     if (user==null)
    //     {
    //         token.token= "No user found";
    //         return token;
    //     }
    //     if(user.Cell.IsNullOrEmpty()){
            
    //         token.token= "No user found";
    //         return token;
    //     }
    //     token.token=GenerateJwtToken(user);
    //     return token;
    // }
    
    // [HttpGet]
    // public User checkLogin(string? cell,string password){
    //   MySqlDataReader dr;
    //   User user=new User();
    //   using (MySqlConnection con = new MySqlConnection(connect))
    //     {
    //         con.Open();
    //         using (MySqlCommand cmd = new MySqlCommand("getUsers", con))
    //         {
    //             cmd.CommandType = CommandType.StoredProcedure;
    //             cmd.Parameters.AddWithValue("@Cell", cell);
    //             cmd.Parameters.AddWithValue("@Password", password);
    //             cmd.Parameters.AddWithValue("@selector", "getlogin");
                
    //             //open reader
    //             dr = cmd.ExecuteReader();
    //             //fill data in datatable
    //             if (dr.Read())
    //             {
    //                user=new User();
    //                user.Name = dr["Name"].ToString();
    //                user.Surname = dr["Surname"].ToString();
    //                user.Cell = dr["Cell"].ToString();
    //                user.Ward = dr["Ward"].ToString();
    //                user.Voting_district = dr["Voting_District"].ToString();
    //                user.Role=dr["role"].ToString();
    //                user.Delegation = dr["Delegation"].ToString();
    //                user.Region = dr["Region"].ToString();
    //                user.Province = dr["Province"].ToString();
    //                user.Municipality = dr["Municipality"].ToString();
    //             }
    //             //close connections
    //             dr.Close();
    //             con.Close();
    //         }
    //     }
    //   return user;
    // }
    
    // private string GenerateJwtToken(User user)
    // {
        
    //     List<Claim> claims=new List<Claim>{
    //         new Claim("Name",user.Name!),
    //         new Claim("Surname",user.Surname!),
    //         new Claim("Role",user.Role!),
    //         new Claim("Delegation",user.Delegation!),
    //         new Claim("Cell",user.Cell!),
    //         new Claim("Ward",user.Ward!),
    //         new Claim("VD",user.Voting_district!),
    //         new Claim("Region",user.Region!),
    //         new Claim("Municipality",user.Municipality!),
    //         new Claim("Province",user.Province!)
    //     };
    //     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt:SecretKey").Value!));

    //     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //     var token = new JwtSecurityToken(
    //         claims:claims,
    //         expires:DateTime.Now.AddDays(1),
    //         signingCredentials:creds
    //     );
    //     var jwt =new JwtSecurityTokenHandler().WriteToken(token);

    //     return jwt;
    // }
}
