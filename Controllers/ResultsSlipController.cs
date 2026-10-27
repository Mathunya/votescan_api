using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MySql.Data.MySqlClient;
using System.Data;
using Web_Api.Models;
using Web_Api.Services;
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

    //READ A PHOTOGRAPHED SLIP - returns the VD and each party's votes.
    //Login required and capped per user per day: every scan is a paid Claude call.
    [Authorize]
    [EnableRateLimiting(RateLimits.SlipScan)]
    [HttpPost]
    [Route("scan")]
    public async Task<IActionResult> scanSlip(
        [FromBody] SlipImage slip,
        [FromServices] ResultsSlipReader reader)
    {
        var (bytes, mimeType, error) = ImageQuotaService.DecodeAndValidate(slip.Image, slip.MediaType);
        if (error != null || bytes == null || mimeType == null)
        {
            return BadRequest(error ?? "No image");
        }
        var cell = User.FindFirst("Cell")?.Value ?? "";
        if (CountScansToday(cell) >= MaxScansPerDay())
        {
            return StatusCode(429, $"Daily limit of {MaxScansPerDay()} slip scans reached.");
        }
        LogScan(cell);
        try
        {
            var result = await reader.ReadAsync(Convert.ToBase64String(bytes), mimeType);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine("results slip scan error => " + ex.Message);
            return StatusCode(502, "Could not read the slip. Please retake the photo.");
        }
    }

    //BACKUP COPY OF A SLIP PHOTO - kept in the database so every API node sees it
    //and deploys never remove it
    [Authorize]
    [EnableRateLimiting(RateLimits.Sensitive)]
    [HttpPost]
    [Route("image")]
    public IActionResult saveImage([FromBody] SlipImage slip)
    {
        var (bytes, mimeType, error) = ImageQuotaService.DecodeAndValidate(slip.Image, slip.MediaType);
        if (error != null || bytes == null || mimeType == null)
        {
            return BadRequest(error ?? "No image");
        }
        if (string.IsNullOrEmpty(slip.VD))
        {
            return BadRequest("No VD");
        }
        long id;
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(@"
                INSERT INTO ResultsSlipImages (VD, Election, Day, Image, ImageMimeType, UploadedBy)
                VALUES (@VD, @Election, @Day, @Image, @ImageMimeType, @UploadedBy);
                SELECT LAST_INSERT_ID();", con))
            {
                cmd.Parameters.AddWithValue("@VD", slip.VD);
                cmd.Parameters.AddWithValue("@Election", slip.Election ?? "");
                cmd.Parameters.AddWithValue("@Day", slip.Day ?? "");
                cmd.Parameters.AddWithValue("@Image", bytes);
                cmd.Parameters.AddWithValue("@ImageMimeType", mimeType);
                cmd.Parameters.AddWithValue("@UploadedBy", User.FindFirst("Cell")?.Value ?? "");
                id = Convert.ToInt64(cmd.ExecuteScalar());
            }
        }
        return Ok(new { id });
    }

    //SLIP PHOTOS SAVED FOR A VD, newest first (details only, no image data)
    [Authorize]
    [HttpGet]
    [Route("images/{vd}")]
    public IActionResult getImages(string vd)
    {
        var all = new List<object>();
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(@"
                SELECT Id, VD, Election, Day, UploadedBy, CreatedAt FROM ResultsSlipImages
                WHERE VD = @VD ORDER BY CreatedAt DESC", con))
            {
                cmd.Parameters.AddWithValue("@VD", vd);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        all.Add(new
                        {
                            id = Convert.ToInt64(dr["Id"]),
                            vd = dr["VD"].ToString(),
                            election = dr["Election"].ToString(),
                            day = dr["Day"].ToString(),
                            uploadedBy = dr["UploadedBy"].ToString(),
                            createdAt = dr["CreatedAt"].ToString(),
                        });
                    }
                }
            }
        }
        return Ok(all);
    }

    //ONE SAVED SLIP PHOTO
    [Authorize]
    [HttpGet]
    [Route("imagefile/{id:long}")]
    public IActionResult getImageFile(long id)
    {
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(
                "SELECT Image, ImageMimeType FROM ResultsSlipImages WHERE Id = @Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                    {
                        return NotFound();
                    }
                    return File((byte[])dr["Image"], dr["ImageMimeType"].ToString() ?? "image/jpeg");
                }
            }
        }
    }

    private int MaxScansPerDay() =>
        int.TryParse(_config?["ResultsSlip:MaxScansPerDay"], out var max) ? max : 20;

    private int CountScansToday(string cell)
    {
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM ResultsSlipScans WHERE Cell = @Cell AND CreatedAt >= CURDATE()", con))
            {
                cmd.Parameters.AddWithValue("@Cell", cell);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }

    private void LogScan(string cell)
    {
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(
                "INSERT INTO ResultsSlipScans (Cell) VALUES (@Cell)", con))
            {
                cmd.Parameters.AddWithValue("@Cell", cell);
                cmd.ExecuteNonQuery();
            }
        }
    }
}

public class SlipImage
{
    public string? Image { get; set; }
    public string? MediaType { get; set; }
    public string? VD { get; set; }
    public string? Election { get; set; }
    public string? Day { get; set; }
}
