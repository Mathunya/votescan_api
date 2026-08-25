using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MySql.Data.MySqlClient;


namespace Web_Api.Controllers;

[Route("[controller]")]
[ApiController]
public class SignalRController : ControllerBase
{
    private  IHubContext<SessionHub>  _hubContext;
    private string? _connect;
     private readonly IConfiguration _config;
    private readonly ILogger<SignalRController> _logger;
    public SignalRController(IHubContext<SessionHub> hubContext,IConfiguration configuration, ILogger<SignalRController> logger)
    {
        _config = configuration;
        _logger = logger;
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _connect = _config.GetConnectionString("ConsString");
      
    }
    [HttpPost("cancel-session")]
    public async Task<IActionResult> LogoutUser(string userId)
    {
        // 1. Update DB
        // UPDATE UserSessions SET IsActive = 0 WHERE UserId = @userId
        await UpdateSessions(userId);

        // 2. Notify user instantly
        await _hubContext.Clients.Group(userId)
            .SendAsync("ForceLogout");

        return Ok();
    }
    private async Task UpdateSessions(string userId)
    {
        try
        {
            using var con = new MySqlConnection(_connect);
            await con.OpenAsync();

            var cmd = new MySqlCommand(@"
            UPDATE UserSessions SET IsActive=0 WHERE UserId=@userId", con);

            cmd.Parameters.AddWithValue("@userId", userId);

            cmd.AddMissingStoredProcedureParameters();

            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update sessions for user {UserId}", userId);
            throw;
        }
    }
}
