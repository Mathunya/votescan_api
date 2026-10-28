using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MySql.Data.MySqlClient;
using System.Data;
using Web_Api.Models;
using Web_Api.Services;
using System.Collections;
using System.Text.Json;
using System.Text.Json.Nodes;

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
            using (MySqlCommand cmd = new MySqlCommand("SELECT Election, Election_date, VD, Municipality, Ward, Candidate_name, Party_name, Votes, Party_agent, Day FROM ResultsSlip WHERE IsLive = 1", con))
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
            using (MySqlCommand cmd = new MySqlCommand("SELECT Election, Election_date, VD, Municipality, Ward, Candidate_name, Party_name, Votes, Party_agent, Day FROM ResultsSlip WHERE Day = @Day AND VD = @VD AND IsLive = 1", con))
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
            using (MySqlCommand cmd = new MySqlCommand("SELECT Election, Election_date, VD, Municipality, Ward, Candidate_name, Party_name, Votes, Party_agent, Day FROM ResultsSlip WHERE Election = @Election AND IsLive = 1", con))
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
            using (MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT Day, Election, VD FROM ResultsSlip WHERE VD = @VD AND IsLive = 1", con))
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
            using (MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT VD FROM ResultsSlip WHERE Election = @Election AND IsLive = 1", con))
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

    //READ A PHOTOGRAPHED SLIP - keeps the photo first, then has Claude read it, then saves
    //the reading as unconfirmed results so nothing is lost if the worker never presses Save.
    //The VD comes from the slip, not the uploader: helpers may upload any VD's slip.
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
        var election = ElectionName(slip.Election);
        //the photo is stored before anything else, so an upload is never lost
        var slipId = SaveUpload(bytes, mimeType, cell, election, slip.VD ?? "");
        //every scan Anthropic answered counts towards the daily limit, including refusals
        //and unreadable answers, since those calls are still paid for; failures where
        //nothing came back (network, timeouts, Anthropic errors) do not count
        JsonElement result;
        try
        {
            result = await reader.ReadAsync(Convert.ToBase64String(bytes), mimeType);
            LogScan(cell);
        }
        catch (SlipNotReadException ex)
        {
            LogScan(cell);
            MarkUpload(slipId, "unreadable");
            Console.WriteLine("results slip scan unusable => " + ex.Message);
            return StatusCode(502, "Could not read the slip. Please retake the photo.");
        }
        catch (Exception ex)
        {
            MarkUpload(slipId, "error");
            Console.WriteLine("results slip scan error => " + ex.Message);
            return StatusCode(502, "Could not read the slip. Please retake the photo.");
        }

        var reading = JsonNode.Parse(result.GetRawText())!.AsObject();
        var vd = DigitsOnly(reading["vd_number"]?.GetValue<string>());
        var ballot = NormaliseBallot(reading["ballot_type"]?.GetValue<string>());
        var info = VdInfo(vd);
        //the next page of a slip: later pages may not repeat the heading, so the VD and
        //ballot of the page before are used when this page does not show them
        var previousVd = DigitsOnly(slip.PageOfVD);
        if (!info.Known && previousVd.Length > 0)
        {
            var previous = VdInfo(previousVd);
            if (previous.Known)
            {
                vd = previousVd;
                info = previous;
                reading["vd_from_previous_page"] = true;
            }
        }
        if (ballot == "unknown" && !string.IsNullOrWhiteSpace(slip.PageOfBallot))
        {
            ballot = NormaliseBallot(slip.PageOfBallot);
        }
        var (pageNo, pageCount) = ParsePage(reading["page"]?.GetValue<string>());
        MarkUpload(slipId, "read", info.Known ? vd : "", ballot, reading.ToJsonString(), pageNo, pageCount);

        //saved straight away as unconfirmed; Save on the phone confirms it later
        var autoSaved = 0;
        var parties = PartiesFromReading(reading);
        if (info.Known && parties.Count > 0)
        {
            try
            {
                autoSaved = SaveResults(election, vd, ballot, parties, "auto", cell, slipId, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("results slip auto-save error => " + ex.Message);
            }
        }

        reading["slip_id"] = slipId;
        reading["ballot"] = ballot;
        reading["page_no"] = pageNo;
        reading["page_count"] = pageCount;
        reading["election"] = election;
        reading["vd_info"] = info.ToJson(vd);
        reading["auto_saved"] = autoSaved;
        reading["existing"] = ExistingSummary(election, vd, ballot);
        return Ok(reading);
    }

    //CONFIRM A SLIP - the worker checked the numbers (and the VD) and pressed Save.
    //Replaces the slip's live results for each party; earlier rows are kept as replaced.
    //Answers 409 when someone already confirmed results for this VD and ballot, unless
    //Replace is true (the app asks the worker first).
    [Authorize]
    [EnableRateLimiting(RateLimits.Sensitive)]
    [HttpPost]
    [Route("confirm")]
    public IActionResult confirmSlip([FromBody] SlipConfirm confirm)
    {
        var vd = DigitsOnly(confirm.VD);
        var info = VdInfo(vd);
        if (!info.Known)
        {
            return BadRequest($"VD {vd} is not on the voters roll. Check the VD number on the slip.");
        }
        var parties = (confirm.Parties ?? [])
            .Where(p => !string.IsNullOrWhiteSpace(p.Name) || !string.IsNullOrWhiteSpace(p.Abbreviation))
            .ToList();
        if (parties.Count == 0)
        {
            return BadRequest("No party results to save.");
        }
        var cell = User.FindFirst("Cell")?.Value ?? "";
        var election = ElectionName(confirm.Election);
        var ballot = NormaliseBallot(confirm.Ballot);
        var existing = ExistingSummary(election, vd, ballot);
        if (!confirm.Replace && existing["confirmed"]!.GetValue<int>() > 0)
        {
            return Conflict(existing);
        }
        var saved = SaveResults(election, vd, ballot, parties, "confirmed", cell, confirm.SlipId,
            confirm.PartyAgent);
        if (confirm.SlipId.HasValue)
        {
            MarkUpload(confirm.SlipId.Value, "saved", vd, ballot, null);
        }
        return Ok(new { saved, vd, ballot, election });
    }

    //LIVE RESULTS FOR ONE VD (current election), one row per ballot and party
    [Authorize]
    [HttpGet]
    [Route("live/{vd}")]
    public IActionResult liveResults(string vd, [FromQuery] string? election)
    {
        var showSaver = IsAdmin();
        var all = new List<object>();
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(@"
                SELECT Ballot, Kind, Party_name, Candidate_name, Votes, Status, SavedBy, SavedAt, SlipImageId
                FROM ResultsSlip
                WHERE VD = @VD AND Election = @Election AND IsLive = 1 AND Ballot IS NOT NULL
                ORDER BY Ballot, Kind, Votes DESC", con))
            {
                cmd.Parameters.AddWithValue("@VD", DigitsOnly(vd));
                cmd.Parameters.AddWithValue("@Election", ElectionName(election));
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        all.Add(new
                        {
                            ballot = dr["Ballot"].ToString(),
                            kind = dr["Kind"].ToString(),
                            party = dr["Party_name"].ToString(),
                            name = dr["Candidate_name"].ToString(),
                            votes = dr["Votes"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Votes"]),
                            status = dr["Status"].ToString(),
                            savedBy = showSaver ? dr["SavedBy"].ToString() : MaskCell(dr["SavedBy"].ToString()),
                            savedAt = dr["SavedAt"] == DBNull.Value ? null
                                : DateTime.SpecifyKind(Convert.ToDateTime(dr["SavedAt"]), DateTimeKind.Utc).ToString("o"),
                            slipId = dr["SlipImageId"] == DBNull.Value ? (long?)null : Convert.ToInt64(dr["SlipImageId"]),
                        });
                    }
                }
            }
        }
        //pages captured per ballot, so a missing page of a multi-page slip shows up
        var pages = new JsonObject();
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(@"
                SELECT Ballot, GROUP_CONCAT(DISTINCT PageNo ORDER BY PageNo) AS Seen, MAX(PageCount) AS PageCount
                FROM ResultsSlipImages
                WHERE VD = @VD AND Election = @Election AND Status IN ('read', 'saved') AND PageNo > 0
                GROUP BY Ballot", con))
            {
                cmd.Parameters.AddWithValue("@VD", DigitsOnly(vd));
                cmd.Parameters.AddWithValue("@Election", ElectionName(election));
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var seen = new JsonArray();
                        foreach (var page in (dr["Seen"].ToString() ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries))
                        {
                            seen.Add(int.Parse(page));
                        }
                        pages[dr["Ballot"].ToString() ?? ""] = new JsonObject
                        {
                            ["seen"] = seen,
                            ["of"] = Convert.ToInt32(dr["PageCount"]),
                        };
                    }
                }
            }
        }
        return Ok(new { results = all, pages });
    }

    //NAME, WARD AND MUNICIPALITY OF A VD, to check a VD number read off (or typed from) a slip
    [Authorize]
    [HttpGet]
    [Route("vdinfo/{vd}")]
    public IActionResult vdInfo(string vd)
    {
        var number = DigitsOnly(vd);
        return Ok(VdInfo(number).ToJson(number));
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

    //SLIP PHOTOS SAVED FOR A VD, newest first (details only, no image data).
    //Only for users whose area covers the VD; uploader numbers only for admins.
    [Authorize]
    [HttpGet]
    [Route("images/{vd}")]
    public IActionResult getImages(string vd)
    {
        if (!CanSeeVd(vd))
        {
            return StatusCode(403, "This VD is outside your area.");
        }
        var showUploader = IsAdmin();
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
                            uploadedBy = showUploader ? dr["UploadedBy"].ToString() : null,
                            createdAt = dr["CreatedAt"].ToString(),
                        });
                    }
                }
            }
        }
        return Ok(all);
    }

    //ONE SAVED SLIP PHOTO, same area check as images/{vd}
    [Authorize]
    [HttpGet]
    [Route("imagefile/{id:long}")]
    public IActionResult getImageFile(long id)
    {
        byte[] image;
        string mimeType;
        string vd;
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(
                "SELECT Image, ImageMimeType, VD FROM ResultsSlipImages WHERE Id = @Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                    {
                        return NotFound();
                    }
                    image = (byte[])dr["Image"];
                    mimeType = dr["ImageMimeType"].ToString() ?? "image/jpeg";
                    vd = dr["VD"].ToString() ?? "";
                }
            }
        }
        if (!CanSeeVd(vd))
        {
            return StatusCode(403, "This VD is outside your area.");
        }
        return File(image, mimeType);
    }

    //roles that may see every VD's slips and who uploaded them
    private static readonly string[] AdminRoles = ["super user", "elections admin", "elections manager"];
    //municipality codes in each Free State region
    private static readonly Dictionary<string, string[]> RegionMunicipalities = new()
    {
        ["mangaung"] = ["MAN"],
        ["xhariep"] = ["FS161", "FS162", "FS163"],
        ["lejweleputswa"] = ["FS181", "FS182", "FS183", "FS184", "FS185"],
        ["thabo mofutsanyana"] = ["FS191", "FS192", "FS193", "FS194", "FS195", "FS196"],
        ["fezile dabi"] = ["FS201", "FS203", "FS204", "FS205"],
    };

    private string Claim(string name) => User.FindFirst(name)?.Value?.Trim() ?? "";

    private bool IsAdmin() =>
        AdminRoles.Contains(Claim("Role").ToLowerInvariant());

    //election structures whose leaders may see slips across their area
    private static readonly string[] StructurePrefixes =
        ["vd ", "bet ", "ward ", "let ", "zet ", "ret ", "rec ", "rtt ", "pet ", "pec "];
    private static readonly string[] LeaderWords =
        ["coordinator", "manager", "admin", "mec", "convenor"];

    //e.g. VD Coordinator, Deputy BET Coordinator, LET Admin, RET Managers, PEC MEC,
    //elections coordinator; not Food/Transport/Youth League coordinators, Candidates,
    //Troubleshooters or Members
    private static bool IsStructureLeader(string role)
    {
        var r = role.Trim().ToLowerInvariant();
        if (r == "elections coordinator")
        {
            return true;
        }
        foreach (var deputy in new[] { "deputy ", "dpt " })
        {
            if (r.StartsWith(deputy))
            {
                r = r[deputy.Length..];
            }
        }
        return StructurePrefixes.Any(prefix => r.StartsWith(prefix)) &&
            LeaderWords.Any(word => r.Contains(word));
    }

    //the user's own VD; structure leaders also see their ward, LET/ZET municipality
    //or RET region (PET/PEC leaders and admins see every VD)
    private bool CanSeeVd(string vd)
    {
        if (string.IsNullOrEmpty(vd))
        {
            return false;
        }
        if (Claim("VD") == vd.Trim() || IsAdmin())
        {
            return true;
        }
        if (!IsStructureLeader(Claim("Role")))
        {
            return false;
        }
        var delegation = Claim("Delegation").ToUpperInvariant();
        if (delegation is "PET" or "PEC")
        {
            return true;
        }
        var (ward, municipality) = VdArea(vd);
        if (ward != "" && Claim("Ward") == ward)
        {
            return true;
        }
        var vdMunicipality = MunicipalityCode(municipality);
        if (delegation is "LET" or "ZET" && vdMunicipality != "" &&
            MunicipalityCode(Claim("Municipality")) == vdMunicipality)
        {
            return true;
        }
        if (delegation is "RET" or "REC" or "RTT" && vdMunicipality != "")
        {
            var region = Claim("Region").ToLowerInvariant();
            return RegionMunicipalities.Any(r => region.Contains(r.Key.Split(' ')[0]) &&
                r.Value.Contains(vdMunicipality));
        }
        return false;
    }

    //"MAN - Mangaung" or "FS184 - Matjhabeng" -> MAN / FS184
    private static string MunicipalityCode(string municipality) =>
        municipality.Split('-')[0].Trim().ToUpperInvariant();

    //ward and municipality of a VD, from the voters roll
    private (string Ward, string Municipality) VdArea(string vd)
    {
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(
                "SELECT WardID, Municipality FROM AllVoters WHERE VDNumber = @VD LIMIT 1", con))
            {
                cmd.Parameters.AddWithValue("@VD", vd);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    return dr.Read()
                        ? (dr["WardID"].ToString()?.Trim() ?? "", dr["Municipality"].ToString()?.Trim() ?? "")
                        : ("", "");
                }
            }
        }
    }

    private static string DigitsOnly(string? value) =>
        new string((value ?? "").Where(char.IsDigit).ToArray());

    //ward | pr | district | unknown
    private static string NormaliseBallot(string? ballot)
    {
        var b = (ballot ?? "").Trim().ToLowerInvariant();
        return b is "ward" or "pr" or "district" ? b : "unknown";
    }

    //"…1234": enough to recognise a colleague, not a full cell number
    private static string? MaskCell(string? cell) =>
        string.IsNullOrEmpty(cell) ? cell : cell.Length <= 4 ? cell : "…" + cell[^4..];

    //the election the app sent, if it exists; otherwise the active one
    private string ElectionName(string? requested)
    {
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(@"
                SELECT name FROM Elections
                WHERE (@Name <> '' AND name = @Name) OR status = 'active'
                ORDER BY (name = @Name) DESC, id DESC LIMIT 1", con))
            {
                cmd.Parameters.AddWithValue("@Name", (requested ?? "").Trim());
                return cmd.ExecuteScalar()?.ToString() ?? (requested ?? "");
            }
        }
    }

    //the proclaimed election date; results slips are all from that day
    private static DateTime? ElectionDate(MySqlConnection con, MySqlTransaction tx, string election)
    {
        using (MySqlCommand cmd = new MySqlCommand(
            "SELECT DATE(COALESCE(ProclaimedDate, end_date)) FROM Elections WHERE name = @Name LIMIT 1", con, tx))
        {
            cmd.Parameters.AddWithValue("@Name", election);
            var value = cmd.ExecuteScalar();
            return value == null || value == DBNull.Value ? null : Convert.ToDateTime(value);
        }
    }

    private record VdDetails(bool Known, string Name, string Ward, string Municipality)
    {
        public JsonObject ToJson(string vd) => new()
        {
            ["vd"] = vd,
            ["known"] = Known,
            ["name"] = Name,
            ["ward"] = Ward,
            ["municipality"] = Municipality,
        };
    }

    //is this VD on the voters roll, and where is it
    private VdDetails VdInfo(string vd)
    {
        if (vd.Length == 0)
        {
            return new VdDetails(false, "", "", "");
        }
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(
                "SELECT VDName, WardID, Municipality FROM AllVoters WHERE VDNumber = @VD LIMIT 1", con))
            {
                cmd.Parameters.AddWithValue("@VD", vd);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    return dr.Read()
                        ? new VdDetails(true, dr["VDName"].ToString()?.Trim() ?? "",
                            dr["WardID"].ToString()?.Trim() ?? "", dr["Municipality"].ToString()?.Trim() ?? "")
                        : new VdDetails(false, "", "", "");
                }
            }
        }
    }

    //keep the photo as soon as it arrives; VD and ballot are filled in after the read
    private long SaveUpload(byte[] bytes, string mimeType, string cell, string election, string userVd)
    {
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(@"
                INSERT INTO ResultsSlipImages (VD, Election, Day, Ballot, Status, Image, ImageMimeType, UploadedBy, UserVD)
                VALUES ('', @Election, '', '', 'uploaded', @Image, @ImageMimeType, @UploadedBy, @UserVD);
                SELECT LAST_INSERT_ID();", con))
            {
                cmd.Parameters.AddWithValue("@Election", election);
                cmd.Parameters.AddWithValue("@Image", bytes);
                cmd.Parameters.AddWithValue("@ImageMimeType", mimeType);
                cmd.Parameters.AddWithValue("@UploadedBy", cell);
                cmd.Parameters.AddWithValue("@UserVD", DigitsOnly(userVd));
                return Convert.ToInt64(cmd.ExecuteScalar());
            }
        }
    }

    //"Page 1 of 2" -> (1, 2); (0, 0) when the slip shows no page number
    private static (int PageNo, int PageCount) ParsePage(string? page)
    {
        var match = System.Text.RegularExpressions.Regex.Match(page ?? "", @"(\d+)\s*(?:of|/)\s*(\d+)",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (!match.Success)
        {
            return (0, 0);
        }
        var no = int.Parse(match.Groups[1].Value);
        var count = int.Parse(match.Groups[2].Value);
        return no >= 1 && count >= no && count <= 20 ? (no, count) : (0, 0);
    }

    //record what happened to an upload; VD/ballot/reading/page only change when given
    private void MarkUpload(long id, string status, string? vd = null, string? ballot = null, string? reading = null,
        int? pageNo = null, int? pageCount = null)
    {
        try
        {
            using (MySqlConnection con = new MySqlConnection(connect))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(@"
                    UPDATE ResultsSlipImages SET
                        Status = @Status,
                        VD = COALESCE(@VD, VD),
                        Ballot = COALESCE(@Ballot, Ballot),
                        Reading = COALESCE(@Reading, Reading),
                        ReadAt = IF(@Reading IS NULL, ReadAt, UTC_TIMESTAMP()),
                        PageNo = COALESCE(@PageNo, PageNo),
                        PageCount = COALESCE(@PageCount, PageCount)
                    WHERE Id = @Id", con))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@VD", (object?)vd ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Ballot", (object?)ballot ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Reading", (object?)reading ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PageNo", (object?)pageNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PageCount", (object?)pageCount ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("results slip upload status error => " + ex.Message);
        }
    }

    private static List<SlipParty> PartiesFromReading(JsonObject reading)
    {
        var parties = new List<SlipParty>();
        if (reading["parties"] is not JsonArray array)
        {
            return parties;
        }
        foreach (var item in array.OfType<JsonObject>())
        {
            parties.Add(new SlipParty
            {
                Name = item["name"]?.GetValue<string>() ?? "",
                Abbreviation = item["abbreviation"]?.GetValue<string>() ?? "",
                Votes = item["votes"]?.GetValue<int>() ?? 0,
            });
        }
        return parties.Where(p => p.Name != "" || p.Abbreviation != "").ToList();
    }

    //the abbreviation is what slips have always been saved under (ANC, DA, EFF)
    private static string PartyKey(SlipParty party) =>
        !string.IsNullOrWhiteSpace(party.Abbreviation) && party.Abbreviation.Trim() != "NO_ABBR"
            ? party.Abbreviation.Trim()
            : (party.Name ?? "").Trim();

    //DUPLICATE GUARD: one live row per election + VD + ballot + party (unique key
    //uq_live_result). Each save marks the party's previous live row as replaced and adds
    //the new one, in one transaction. An automatic save never replaces a number a person
    //has confirmed. Galera can reject one of two saves racing on different nodes
    //(1213/1062); that save is retried once and then replaces the other.
    private int SaveResults(string election, string vd, string ballot, List<SlipParty> parties,
        string status, string cell, long? slipId, string? agent)
    {
        var (ward, municipality) = VdArea(vd);
        for (var attempt = 0; ; attempt++)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connect))
                {
                    con.Open();
                    using (MySqlTransaction tx = con.BeginTransaction())
                    {
                        var day = ElectionDate(con, tx, election);
                        var saved = 0;
                        foreach (var party in parties.GroupBy(PartyKey).Select(g => g.Last()))
                        {
                            var key = PartyKey(party);
                            if (key == "")
                            {
                                continue;
                            }
                            if (status == "auto")
                            {
                                using (MySqlCommand check = new MySqlCommand(@"
                                    SELECT COUNT(*) FROM ResultsSlip
                                    WHERE Election = @Election AND VD = @VD AND Ballot = @Ballot AND Kind = 'slip'
                                      AND Party_name = @Party AND IsLive = 1 AND Status = 'confirmed'", con, tx))
                                {
                                    check.Parameters.AddWithValue("@Election", election);
                                    check.Parameters.AddWithValue("@VD", vd);
                                    check.Parameters.AddWithValue("@Ballot", ballot);
                                    check.Parameters.AddWithValue("@Party", key);
                                    if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                                    {
                                        continue;
                                    }
                                }
                            }
                            using (MySqlCommand replace = new MySqlCommand(@"
                                UPDATE ResultsSlip SET IsLive = NULL, ReplacedAt = UTC_TIMESTAMP(), ReplacedBy = @Cell
                                WHERE Election = @Election AND VD = @VD AND Ballot = @Ballot AND Kind = 'slip'
                                  AND Party_name = @Party AND IsLive = 1", con, tx))
                            {
                                replace.Parameters.AddWithValue("@Cell", cell);
                                replace.Parameters.AddWithValue("@Election", election);
                                replace.Parameters.AddWithValue("@VD", vd);
                                replace.Parameters.AddWithValue("@Ballot", ballot);
                                replace.Parameters.AddWithValue("@Party", key);
                                replace.ExecuteNonQuery();
                            }
                            using (MySqlCommand insert = new MySqlCommand(@"
                                INSERT INTO ResultsSlip
                                    (Election, Election_date, VD, Municipality, Ward, Candidate_name, Party_name, Votes,
                                     Party_agent, Day, Ballot, Kind, Status, SlipImageId, SavedBy, SavedAt, IsLive)
                                VALUES
                                    (@Election, @Day, @VD, @Municipality, @Ward, @Name, @Party, @Votes,
                                     @Agent, @Day, @Ballot, 'slip', @Status, @SlipId, @Cell, UTC_TIMESTAMP(), 1)", con, tx))
                            {
                                insert.Parameters.AddWithValue("@Election", election);
                                insert.Parameters.AddWithValue("@Day", (object?)day ?? DBNull.Value);
                                insert.Parameters.AddWithValue("@VD", vd);
                                insert.Parameters.AddWithValue("@Municipality", municipality);
                                insert.Parameters.AddWithValue("@Ward", ward);
                                insert.Parameters.AddWithValue("@Name", party.Name ?? "");
                                insert.Parameters.AddWithValue("@Party", key);
                                insert.Parameters.AddWithValue("@Votes", Math.Max(0, party.Votes));
                                insert.Parameters.AddWithValue("@Agent", agent ?? "");
                                insert.Parameters.AddWithValue("@Ballot", ballot);
                                insert.Parameters.AddWithValue("@Status", status);
                                insert.Parameters.AddWithValue("@SlipId", (object?)slipId ?? DBNull.Value);
                                insert.Parameters.AddWithValue("@Cell", cell);
                                insert.ExecuteNonQuery();
                            }
                            saved++;
                        }
                        tx.Commit();
                        return saved;
                    }
                }
            }
            catch (MySqlException ex) when (attempt == 0 && (ex.Number == 1213 || ex.Number == 1062))
            {
                Console.WriteLine("results slip save retry => " + ex.Message);
            }
        }
    }

    //what is already live for this VD and ballot: how many parties confirmed / automatic,
    //and who saved last (masked unless admin)
    private JsonObject ExistingSummary(string election, string vd, string ballot)
    {
        var summary = new JsonObject { ["confirmed"] = 0, ["auto"] = 0, ["savedBy"] = null, ["savedAt"] = null };
        if (vd.Length == 0)
        {
            return summary;
        }
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(@"
                SELECT
                    SUM(Status = 'confirmed') AS Confirmed,
                    SUM(Status = 'auto') AS Auto,
                    SUBSTRING_INDEX(GROUP_CONCAT(SavedBy ORDER BY Status = 'confirmed' DESC, SavedAt DESC), ',', 1) AS SavedBy,
                    MAX(SavedAt) AS SavedAt
                FROM ResultsSlip
                WHERE Election = @Election AND VD = @VD AND Ballot = @Ballot AND Kind = 'slip' AND IsLive = 1", con))
            {
                cmd.Parameters.AddWithValue("@Election", election);
                cmd.Parameters.AddWithValue("@VD", vd);
                cmd.Parameters.AddWithValue("@Ballot", ballot);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read() && dr["Confirmed"] != DBNull.Value)
                    {
                        summary["confirmed"] = Convert.ToInt32(dr["Confirmed"]);
                        summary["auto"] = Convert.ToInt32(dr["Auto"]);
                        var savedBy = dr["SavedBy"].ToString();
                        summary["savedBy"] = IsAdmin() ? savedBy : MaskCell(savedBy);
                        summary["savedAt"] = dr["SavedAt"] == DBNull.Value ? null
                            : DateTime.SpecifyKind(Convert.ToDateTime(dr["SavedAt"]), DateTimeKind.Utc).ToString("o");
                    }
                }
            }
        }
        return summary;
    }

    private int MaxScansPerDay() =>
        int.TryParse(_config?["ResultsSlip:MaxScansPerDay"], out var max) ? max : 20;

    //scans since midnight South African time (UTC+2); the DB runs on UTC, so CreatedAt
    //compares directly with the UTC instant of SAST midnight
    private int CountScansToday(string cell)
    {
        var sastMidnightUtc = DateTime.UtcNow.AddHours(2).Date.AddHours(-2);
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM ResultsSlipScans WHERE Cell = @Cell AND CreatedAt >= @Since", con))
            {
                cmd.Parameters.AddWithValue("@Cell", cell);
                cmd.Parameters.AddWithValue("@Since", sastMidnightUtc);
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

public class SlipParty
{
    public string? Name { get; set; }
    public string? Abbreviation { get; set; }
    public int Votes { get; set; }
}

public class SlipConfirm
{
    public long? SlipId { get; set; }
    public string? VD { get; set; }
    public string? Ballot { get; set; }
    public string? Election { get; set; }
    public string? PartyAgent { get; set; }
    public List<SlipParty>? Parties { get; set; }
    //true after the worker agreed to replace results someone already confirmed
    public bool Replace { get; set; }
}

public class SlipImage
{
    public string? Image { get; set; }
    public string? MediaType { get; set; }
    public string? VD { get; set; }
    public string? Election { get; set; }
    public string? Day { get; set; }
    //set when this photo is the next page of a slip: that slip's VD and ballot
    public string? PageOfVD { get; set; }
    public string? PageOfBallot { get; set; }
}
