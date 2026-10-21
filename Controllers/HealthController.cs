using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Web_Api.Controllers;

// Liveness check that touches the database, for DNS (PowerDNS ifurlup) and HAProxy health checks.
// GET /health/db -> 200 "ok" when the local database answers, 503 otherwise.
// On a Galera node it also returns 503 unless wsrep_ready is ON, i.e. the node is in the
// cluster majority and may take traffic. On a non-Galera server the wsrep check is skipped.
// No application data is read; no details are returned to the caller.
[Route("[controller]")]
[ApiController]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    private readonly string? connect;
    private readonly ILogger<HealthController> _logger;

    public HealthController(IConfiguration configuration, ILogger<HealthController> logger)
    {
        _logger = logger;
        var cs = configuration.GetConnectionString("ConsString");
        if (cs != null)
        {
            // Fail fast: a health check that hangs is as bad as one that lies.
            var builder = new MySqlConnectionStringBuilder(cs) { ConnectionTimeout = 3, DefaultCommandTimeout = 3 };
            connect = builder.ConnectionString;
        }
    }

    [HttpGet("db")]
    public IActionResult Db()
    {
        Response.Headers.CacheControl = "no-store";
        try
        {
            using (MySqlConnection con = new MySqlConnection(connect))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("SHOW GLOBAL STATUS LIKE 'wsrep_ready'", con))
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read() && dr.GetString(1) != "ON")
                    {
                        _logger.LogWarning("Health check: Galera node not ready (wsrep_ready={State})", dr.GetString(1));
                        return StatusCode(503, "db not ready");
                    }
                }
                using (MySqlCommand cmd = new MySqlCommand("SELECT 1", con))
                {
                    cmd.ExecuteScalar();
                }
            }
            return Ok("ok");
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Health check: database unavailable: {Message}", ex.Message);
            return StatusCode(503, "db unavailable");
        }
    }
}
