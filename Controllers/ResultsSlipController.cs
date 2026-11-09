using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
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

    //one calculation per cache key at a time; requests arriving while a key is being
    //built wait for that calculation instead of each running the same four queries
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, SemaphoreSlim> SummaryGates = new();

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
     //OFFICIAL IEC PR-BALLOT RESULTS for a set of municipalities (LGE 2026 Results
    //Report). One row per VD per party. The frontend rolls these up into
    //province/region/municipality/ward/VD sections itself, same pattern as
    //Reports/getdailycanvassing + getvdroll for the Movement report.
    [HttpGet]
    [Route("iecpr")]
    public IActionResult getIecPr([FromQuery] string municipalities)
    {
        var codes = (municipalities ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (codes.Length == 0)
        {
            return Ok(Array.Empty<object>());
        }
        var all = new List<object>();
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            var placeholders = string.Join(",", codes.Select((c, i) => $"@m{i}"));
            using (MySqlCommand cmd = new MySqlCommand(
                $@"SELECT Municipality, Ward, VD, VotingStationName, RegisteredVoters, PartyName, TotalValidVotes
                   FROM ResultsOfficial
                   WHERE BallotType = 'PR' AND Municipality IN ({placeholders})", con))
            {
                for (int i = 0; i < codes.Length; i++)
                {
                    cmd.Parameters.AddWithValue($"@m{i}", codes[i]);
                }
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        all.Add(new
                        {
                            municipality = dr["Municipality"].ToString(),
                            ward = dr["Ward"].ToString(),
                            vd = dr["VD"].ToString(),
                            vdName = dr["VotingStationName"].ToString(),
                            registeredVoters = dr["RegisteredVoters"] is DBNull ? 0 : Convert.ToInt32(dr["RegisteredVoters"]),
                            party = dr["PartyName"].ToString(),
                            votes = dr["TotalValidVotes"] is DBNull ? 0 : Convert.ToInt32(dr["TotalValidVotes"]),
                        });
                    }
                }
            }
        }
        return Ok(all);
    }

    //OUR OWN ELECTION-DAY SLIP CAPTURES (PR ballot), same municipality scope as
    //getIecPr above, so the LGE Results Report can compare what our slips said
    //to what the IEC's official count says.
    [HttpGet]
    [Route("electionday")]
    public IActionResult getElectionDay([FromQuery] string municipalities, [FromQuery] string election = "LocaL Goverment Elections 2026")
    {
        var codes = (municipalities ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (codes.Length == 0)
        {
            return Ok(Array.Empty<object>());
        }
        var all = new List<object>();
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            var placeholders = string.Join(",", codes.Select((c, i) => $"@m{i}"));
            using (MySqlCommand cmd = new MySqlCommand(
                $@"SELECT Municipality, Ward, VD, Party_name, Votes
                   FROM ResultsSlip
                   WHERE Election = @Election AND IsLive = 1 AND Kind = 'slip' AND LOWER(Ballot) = 'pr'
                   AND Municipality IN ({placeholders})", con))
            {
                cmd.Parameters.AddWithValue("@Election", election);
                for (int i = 0; i < codes.Length; i++)
                {
                    cmd.Parameters.AddWithValue($"@m{i}", codes[i]);
                }
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        all.Add(new
                        {
                            municipality = dr["Municipality"].ToString(),
                            ward = dr["Ward"].ToString(),
                            vd = dr["VD"].ToString(),
                            party = dr["Party_name"].ToString(),
                            votes = dr["Votes"] is DBNull ? 0 : Convert.ToInt32(dr["Votes"]),
                        });
                    }
                }
            }
        }
        return Ok(all);
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
        //every page of the slip is marked saved
        var slipIds = (confirm.SlipIds ?? []).ToList();
        if (confirm.SlipId.HasValue)
        {
            slipIds.Add(confirm.SlipId.Value);
        }
        foreach (var id in slipIds.Distinct())
        {
            MarkUpload(id, "saved", vd, ballot, null);
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

    //RESULTS DASHBOARD SUMMARY FOR ONE MUNICIPALITY - party votes, ward leaders, coverage,
    //flags and a PR seat estimate, all added up already. Cached in this server's memory
    //for 60s per election+municipality+mode so a dashboard refreshed by many people at
    //once costs at most one set of queries a minute per node (see the Results Slip
    //Workflow and Seats plan, section 5). mode=confirmed (default) counts confirmed
    //slips only; mode=all also counts auto (unconfirmed) rows.
    [Authorize]
    [HttpGet]
    [Route("summary/{municipality}")]
    public async Task<IActionResult> summary(string municipality, [FromQuery] string? mode, [FromServices] IMemoryCache cache)
    {
        var all = mode == "all";
        var election = ElectionName(null);
        return Ok(await GetOrBuildSummaryAsync(election, municipality, all, cache));
    }

    //one cache-miss computation per (election, municipality, mode) at a time, awaited
    //rather than blocking a thread-pool thread, shared by summary() and
    //summaryByRegion() so a region request warming several cold municipalities at once
    //still only runs each municipality's four queries once.
    private async Task<JsonObject> GetOrBuildSummaryAsync(string election, string municipality, bool all, IMemoryCache cache)
    {
        var key = $"slip-summary|{election}|{municipality}|{(all ? "all" : "confirmed")}";
        if (cache.TryGetValue(key, out JsonObject? hit))
        {
            return hit!;
        }
        var gate = SummaryGates.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync();
        try
        {
            if (cache.TryGetValue(key, out hit))
            {
                return hit!;
            }
            var result = BuildSummary(election, municipality, all, cache);
            cache.Set(key, result, TimeSpan.FromSeconds(60));
            return result;
        }
        finally
        {
            gate.Release();
        }
    }

    //district-ballot ('district') party votes for a whole region at once - this is
    //the district council's own 40% directly-elected PR ballot, not any municipality's
    //ward/PR ballots, so it's cached per-region rather than reusing the per-municipality
    //cache/gate summary() and summaryByRegion() share.
    private async Task<JsonArray> GetOrBuildDistrictPartiesAsync(
        string election, string region, List<string> municipalities, bool all, IMemoryCache cache)
    {
        var key = $"slip-district|{election}|{region}|{(all ? "all" : "confirmed")}";
        if (cache.TryGetValue(key, out JsonArray? hit))
        {
            return hit!;
        }
        var gate = SummaryGates.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync();
        try
        {
            if (cache.TryGetValue(key, out hit))
            {
                return hit!;
            }
            var result = BuildDistrictParties(election, municipalities, all);
            cache.Set(key, result, TimeSpan.FromSeconds(60));
            return result;
        }
        finally
        {
            gate.Release();
        }
    }

    private JsonArray BuildDistrictParties(string election, List<string> municipalities, bool all)
    {
        var byKey = new Dictionary<string, (string Name, long Votes)>();
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            var placeholders = string.Join(",", municipalities.Select((_, i) => $"@M{i}"));
            using (MySqlCommand cmd = new MySqlCommand($@"
                SELECT Party_name, MAX(Candidate_name) AS Name, SUM(Votes) AS Votes
                FROM ResultsSlip
                WHERE Election = @E AND IsLive = 1 AND Kind = 'slip' AND Ballot = 'district'
                  AND (@All = 1 OR Status = 'confirmed')
                  AND Municipality IN ({placeholders})
                GROUP BY Party_name", con))
            {
                cmd.Parameters.AddWithValue("@E", election);
                cmd.Parameters.AddWithValue("@All", all ? 1 : 0);
                for (var i = 0; i < municipalities.Count; i++)
                {
                    cmd.Parameters.AddWithValue($"@M{i}", municipalities[i]);
                }
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var key = NormalisedPartyKey(dr["Party_name"].ToString());
                        var name = dr["Name"].ToString() ?? "";
                        var votes = Convert.ToInt64(dr["Votes"]);
                        var (existingName, existingVotes) = byKey.TryGetValue(key, out var e) ? e : ("", 0L);
                        byKey[key] = (string.IsNullOrEmpty(existingName) ? name : existingName, existingVotes + votes);
                    }
                }
            }
        }
        return new JsonArray(byKey
            .OrderByDescending(kv => kv.Value.Votes)
            .Select(kv => (JsonNode)new JsonObject
            {
                ["key"] = kv.Key,
                ["name"] = string.IsNullOrEmpty(kv.Value.Name) ? kv.Key : kv.Value.Name,
                ["totalVotes"] = kv.Value.Votes,
            }).ToArray());
    }

    //SAME TOTALS ADDED ACROSS A REGION'S MUNICIPALITIES. Seats are not re-estimated here
    //(the council size is per municipality, so "seats for a region" has no meaning) -
    //only coverage and party votes are summed. Each municipality's own summary goes
    //through the same cache/gate as summary(), so this piggybacks on whichever of them
    //are already warm instead of racing them to recompute a cold one.
    [Authorize]
    [HttpGet]
    [Route("summary/region/{region}")]
    public async Task<IActionResult> summaryByRegion(string region, [FromQuery] string? mode, [FromServices] IMemoryCache cache)
    {
        var all = mode == "all";
        var election = ElectionName(null);
        var municipalities = MunicipalitiesInRegion(region);
        if (municipalities.Count == 0)
        {
            return BadRequest($"Unknown region '{region}'.");
        }
        int vds = 0, vdsWard = 0, vdsPr = 0, vdsBoth = 0, unconfirmedSlips = 0, flaggedSlips = 0;
        long registered = 0, registeredCounted = 0;
        var parties = new Dictionary<string, (string Name, long WardVotes, long PrVotes)>();
        foreach (var municipality in municipalities)
        {
            var muni = await GetOrBuildSummaryAsync(election, municipality, all, cache);
            var c = muni["coverage"]!.AsObject();
            vds += c["vds"]!.GetValue<int>();
            vdsWard += c["vdsWard"]!.GetValue<int>();
            vdsPr += c["vdsPr"]!.GetValue<int>();
            vdsBoth += c["vdsBoth"]!.GetValue<int>();
            registered += c["registered"]!.GetValue<long>();
            registeredCounted += c["registeredCounted"]!.GetValue<long>();
            unconfirmedSlips += c["unconfirmedSlips"]!.GetValue<int>();
            flaggedSlips += c["flaggedSlips"]!.GetValue<int>();
            foreach (var p in muni["parties"]!.AsArray())
            {
                var pKey = p!["key"]!.GetValue<string>();
                var wardVotes = p["wardVotes"]!.GetValue<long>();
                var prVotes = p["prVotes"]!.GetValue<long>();
                var (name, wv, pv) = parties.TryGetValue(pKey, out var existing) ? existing : (p["name"]!.GetValue<string>(), 0L, 0L);
                parties[pKey] = (name, wv + wardVotes, pv + prVotes);
            }
        }
        var districtParties = await GetOrBuildDistrictPartiesAsync(election, region, municipalities, all, cache);
        return Ok(new JsonObject
        {
            ["election"] = election,
            ["region"] = region,
            ["mode"] = all ? "all" : "confirmed",
            ["generatedAt"] = DateTime.UtcNow.ToString("o"),
            ["municipalities"] = new JsonArray(municipalities.Select(m => JsonValue.Create(m)).ToArray()),
            ["coverage"] = new JsonObject
            {
                ["vds"] = vds,
                ["vdsWard"] = vdsWard,
                ["vdsPr"] = vdsPr,
                ["vdsBoth"] = vdsBoth,
                ["registered"] = registered,
                ["registeredCounted"] = registeredCounted,
                ["unconfirmedSlips"] = unconfirmedSlips,
                ["flaggedSlips"] = flaggedSlips,
            },
            ["parties"] = new JsonArray(parties.Select(kv => (JsonNode)new JsonObject
            {
                ["key"] = kv.Key,
                ["name"] = kv.Value.Name,
                ["wardVotes"] = kv.Value.WardVotes,
                ["prVotes"] = kv.Value.PrVotes,
                ["totalVotes"] = kv.Value.WardVotes + kv.Value.PrVotes,
            }).ToArray()),
            // The district council's own 40% directly-elected PR ballot (Ballot =
            // 'district') - party-only, like any PR ballot, and inherently
            // region-wide rather than tied to one municipality, so it's summed once
            // here rather than appearing in any single municipality's own summary.
            // The other 60% of a district council is appointed from local councils'
            // own composition (Schedule 2) - not calculated here, since that needs
            // every local council in the district to have final seat results first,
            // which this system has no reliable way to know this early in counting.
            ["districtParties"] = districtParties,
        });
    }

    //PER-VD, PER-BALLOT STATUS for the results dashboard's VD grid and its "needs
    //attention" list. Unlike the flag *counts* in /summary, this names which VD and
    //ballot each flag belongs to and why, so a flagged slip can actually be found and
    //corrected rather than just counted.
    [Authorize]
    [HttpGet]
    [Route("coverage/{municipality}")]
    public async Task<IActionResult> coverage(string municipality, [FromServices] IMemoryCache cache)
    {
        var election = ElectionName(null);
        var key = $"slip-coverage|{election}|{municipality}";
        if (cache.TryGetValue(key, out JsonArray? hit))
        {
            return Ok(hit);
        }
        var gate = SummaryGates.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync();
        try
        {
            if (cache.TryGetValue(key, out hit))
            {
                return Ok(hit);
            }
            var result = BuildCoverage(election, municipality, cache);
            cache.Set(key, result, TimeSpan.FromSeconds(60));
            return Ok(result);
        }
        finally
        {
            gate.Release();
        }
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
                SELECT Id, VD, Election, Day, Ballot, UploadedBy, CreatedAt FROM ResultsSlipImages
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
                            // the ballot type isn't known until the OCR reads it, so this can
                            // be "" for a just-uploaded, not-yet-read image
                            ballot = dr["Ballot"].ToString(),
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

    //ward | pr | district - defaults to ward when the slip's own heading doesn't say
    //(was "unknown", which excluded the slip from every count on the dashboard until a
    //person corrected it by hand; most slips read this way are in fact ward ballots)
    private static string NormaliseBallot(string? ballot)
    {
        var b = (ballot ?? "").Trim().ToLowerInvariant();
        return b is "pr" or "district" ? b : "ward";
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

    //the OCR reads a ward slip's abbreviation column ("ANC") but a PR slip has no
    //abbreviation column at all, only the full party name ("AFRICAN NATIONAL
    //CONGRESS") — so PartyKey() falls back to that full name for PR rows. Without
    //this map, a party's ward votes and PR votes land under two entirely different
    //keys and never combine into one party anywhere (Party votes, ward winners, the
    //seat estimate). Built from the full names actually seen on real 2026 slips so
    //far rather than typed out from memory; a party not listed here yet (because
    //neither ballot has produced one of these two forms for it yet, or its full
    //name was OCR'd slightly differently) still won't merge until it's added.
    private static readonly Dictionary<string, string> PartyFullNameToAbbreviation = new()
    {
        ["AFRICAN NATIONAL CONGRESS"] = "ANC",
        ["DEMOCRATIC ALLIANCE"] = "DA",
        ["ECONOMIC FREEDOM FIGHTERS"] = "EFF",
        ["INKATHA FREEDOM PARTY"] = "IFP",
        ["VRYHEIDSFRONT PLUS | FREEDOM FRONT PLUS"] = "VF PLUS",
        ["CONGRESS OF THE PEOPLE"] = "COPE",
        ["UNITED CHRISTIAN DEMOCRATIC PARTY"] = "UCDP",
        ["AFRICAN CHRISTIAN DEMOCRATIC PARTY"] = "ACDP",
        ["PAN AFRICANIST CONGRESS OF AZANIA"] = "PAC",
        ["UMKHONTO WESIZWE PARTY"] = "MKP",
        ["BUILD ONE SOUTH AFRICA"] = "BOSA",
        ["PATRIOTIC ALLIANCE"] = "PA",
        ["AFRICAN TRANSFORMATION MOVEMENT"] = "ATM",
        ["AFRICAN PEOPLE'S CONVENTION"] = "APC",
        ["SOUTH AFRICAN COMMUNIST PARTY"] = "SACP",
        ["LABOUR PARTY OF SOUTH AFRICA"] = "LABOUR",
        ["UNITED RESIDENTS FRONT"] = "URF",
        ["AFRICAN ALLIANCE CONGRESS"] = "AAC",
        ["AFRICAN CULTURAL IDENTITY MOVEMENT"] = "ACIM",
        ["AFRICAN HEART CONGRESS"] = "AHC",
        ["AFRICAN INDEPENDENT CONGRESS"] = "AIC",
        ["AFRICAN CONGRESS FOR TRANSFORMATION"] = "ACT",
        ["ACTIVE AFRICAN CHRISTIANS UNITED MOVEMENT"] = "AACUM",
        ["AFRIKA MAYIBUYE MOVEMENT"] = "MAYIBUYE",
        ["AFRIKAN ALLIANCE OF SOCIAL DEMOCRATS"] = "AASD",
        ["MANGAUNG SEVEN PEOPLE'S VOICE"] = "MSPV",
        ["NATIONAL SERVICE DELIVERY FORUM"] = "NSDF",
        ["PEOPLE'S MOVEMENT FOR CHANGE"] = "PMC",
        ["SOUTH AFRICAN ROYAL KINGDOMS ORGANIZATION"] = "SARKO",
        ["SIZWE UMMAH NATION"] = "SUN",
    };

    //a slip abbreviation is free text typed by a person, so "ANC", " anc" and "Anc " are
    //the same party and should not become three near-duplicate rows. This merges
    //casing/whitespace variants, then the full-name alias above, so "ANC" (from a ward
    //slip) and "AFRICAN NATIONAL CONGRESS" (from a PR slip) land on the same key. It
    //cannot tell an independent candidate or a genuinely different (misspelt) party
    //from a typo of a real one; that needs a real per-municipality party list (the
    //plan's "Keep one party list per municipality" change), which does not exist yet.
    private static string NormalisedPartyKey(string? raw)
    {
        var key = (raw ?? "").Trim().ToUpperInvariant();
        return PartyFullNameToAbbreviation.TryGetValue(key, out var abbr) ? abbr : key;
    }

    //municipality codes ("FS192 - Dihlabeng") for a region, discovered from the voters
    //roll rather than hardcoded twice - reuses the same region->prefix map CanSeeVd uses.
    private List<string> MunicipalitiesInRegion(string region)
    {
        var normalised = region.Trim().ToLowerInvariant();
        if (!RegionMunicipalities.TryGetValue(normalised, out var prefixes))
        {
            return [];
        }
        var municipalities = new List<string>();
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            foreach (var prefix in prefixes)
            {
                // MAN -> "MAN - Mangaung"; this grabs the one exact code per prefix
                // rather than guessing the full display name
                using (MySqlCommand lookup = new MySqlCommand(
                    "SELECT DISTINCT Municipality FROM AllVoters WHERE Municipality LIKE @Prefix LIMIT 1", con))
                {
                    lookup.Parameters.AddWithValue("@Prefix", prefix + "%");
                    var found = lookup.ExecuteScalar()?.ToString();
                    if (!string.IsNullOrEmpty(found))
                    {
                        municipalities.Add(found);
                    }
                }
            }
        }
        return municipalities;
    }

    //council size/ward count for a municipality's seat estimate, or null when nobody
    //has entered one yet (CouncilSeats, see results_summary.sql)
    private (int Seats, int Wards, int IndependentWards)? GetCouncilSeats(string municipality)
    {
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(
                "SELECT Seats, Wards, IndependentWards FROM CouncilSeats WHERE Municipality = @M", con))
            {
                cmd.Parameters.AddWithValue("@M", municipality);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    return dr.Read()
                        ? (Convert.ToInt32(dr["Seats"]), Convert.ToInt32(dr["Wards"]), Convert.ToInt32(dr["IndependentWards"]))
                        : null;
                }
            }
        }
    }

    //every VD in a municipality, its ward, and how many registered voters it has.
    //Changes about once a day (voters roll updates), not every 60s like results do, so
    //it's cached separately and for far longer - re-running this GROUP BY over AllVoters
    //on every summary/coverage cache refresh was real, measured load on election night
    //(2,000 req/min, 47% peak CPU) for data that is essentially static in that window.
    private Dictionary<string, (string Ward, long Registered)> GetVdRoster(string municipality, IMemoryCache cache)
    {
        var key = $"vd-roster|{municipality}";
        if (cache.TryGetValue(key, out Dictionary<string, (string Ward, long Registered)>? hit))
        {
            return hit!;
        }
        var roster = new Dictionary<string, (string Ward, long Registered)>();
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(@"
                SELECT VDNumber, WardID, COUNT(*) AS Registered
                FROM AllVoters WHERE Municipality = @M
                GROUP BY VDNumber, WardID", con))
            {
                cmd.Parameters.AddWithValue("@M", municipality);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        roster[dr["VDNumber"].ToString() ?? ""] =
                            (dr["WardID"].ToString() ?? "", Convert.ToInt64(dr["Registered"]));
                    }
                }
            }
        }
        cache.Set(key, roster, TimeSpan.FromHours(24));
        return roster;
    }

    //everything GET /ResultsSlip/summary/{municipality} returns: coverage, party votes,
    //ward leaders, flags and (when CouncilSeats has a row for this municipality) the PR
    //seat estimate. Runs the four queries from the plan's section 5 plus the VD roster;
    //only called on a cache miss.
    private JsonObject BuildSummary(string election, string municipality, bool all, IMemoryCache cache)
    {
        //1 - every VD in the municipality and how many registered voters it has
        var registeredByVd = GetVdRoster(municipality, cache);

        //2 - which VDs have a confirmed/unconfirmed ward and PR slip live right now
        var statusByVdBallot = new Dictionary<(string Vd, string Ballot), (bool Confirmed, bool Unconfirmed)>();
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(@"
                SELECT VD, Ballot,
                       MAX(Status = 'confirmed') AS Confirmed,
                       MAX(Status = 'auto') AS Unconfirmed
                FROM ResultsSlip
                WHERE Election = @E AND Municipality = @M AND IsLive = 1 AND Kind = 'slip' AND Ballot IS NOT NULL
                GROUP BY VD, Ballot", con))
            {
                cmd.Parameters.AddWithValue("@E", election);
                cmd.Parameters.AddWithValue("@M", municipality);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        statusByVdBallot[(dr["VD"].ToString() ?? "", dr["Ballot"].ToString() ?? "")] =
                            (Convert.ToInt32(dr["Confirmed"]) > 0, Convert.ToInt32(dr["Unconfirmed"]) > 0);
                    }
                }
            }
        }

        bool Counted((bool Confirmed, bool Unconfirmed) s) => s.Confirmed || (all && s.Unconfirmed);

        var vds = registeredByVd.Count;
        var registered = registeredByVd.Values.Sum(v => v.Registered);
        var vdsWard = 0; var vdsPr = 0; var vdsBoth = 0; long registeredCounted = 0; var unconfirmedSlips = 0;
        foreach (var (vd, info) in registeredByVd)
        {
            var wardOk = statusByVdBallot.TryGetValue((vd, "ward"), out var w) && Counted(w);
            var prOk = statusByVdBallot.TryGetValue((vd, "pr"), out var p) && Counted(p);
            if (wardOk) vdsWard++;
            if (prOk) vdsPr++;
            if (wardOk && prOk)
            {
                vdsBoth++;
                registeredCounted += info.Registered;
            }
        }
        unconfirmedSlips = statusByVdBallot.Values.Count(s => !s.Confirmed && s.Unconfirmed);

        //3 - flags from the photographed pages: missing pages, illegible numbers, unknown
        //ballot. Grouped the same way coverage() uses, so the counts here always match the
        //reasons a VD shows up with on the needs-attention list.
        var pagesMissing = 0; var illegible = 0; var unknownBallot = 0;
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(@"
                SELECT VD, Ballot, MAX(PageCount) AS PageCount,
                       COUNT(DISTINCT CASE WHEN PageNo > 0 THEN PageNo END) AS Seen,
                       MAX(CASE WHEN Reading IS NOT NULL
                            AND JSON_CONTAINS(JSON_EXTRACT(Reading, '$.parties[*].legible'), 'false')
                            THEN 1 ELSE 0 END) AS Illegible
                FROM ResultsSlipImages
                WHERE Election = @E AND VD IN (SELECT VDNumber FROM AllVoters WHERE Municipality = @M)
                  AND Status IN ('read', 'saved')
                GROUP BY VD, Ballot", con))
            {
                cmd.Parameters.AddWithValue("@E", election);
                cmd.Parameters.AddWithValue("@M", municipality);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var pageCount = Convert.ToInt32(dr["PageCount"]);
                        var seen = Convert.ToInt32(dr["Seen"]);
                        var ballot = dr["Ballot"].ToString() ?? "";
                        if (pageCount > 0 && seen < pageCount) pagesMissing++;
                        if (Convert.ToInt32(dr["Illegible"]) > 0) illegible++;
                        if (ballot == "unknown") unknownBallot++;
                    }
                }
            }
        }

        //4 - replaced results: a second worker confirmed over an earlier confirm. Not
        //wrong by itself, but worth a human glance since it means two different Saves
        //disagreed about this VD and ballot.
        var replaced = 0;
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(@"
                SELECT COUNT(DISTINCT VD, Ballot) FROM ResultsSlip
                WHERE Election = @E AND Municipality = @M AND Kind = 'slip' AND ReplacedAt IS NOT NULL
                  AND Status = 'confirmed'", con))
            {
                cmd.Parameters.AddWithValue("@E", election);
                cmd.Parameters.AddWithValue("@M", municipality);
                replaced = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        //5 - party votes per ballot (query 1 of the plan)
        var partyRows = new List<(string Ballot, string Key, string Name, long Votes)>();
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(@"
                SELECT Ballot, Party_name, MAX(Candidate_name) AS Name, SUM(Votes) AS Votes
                FROM ResultsSlip
                WHERE Election = @E AND Municipality = @M AND IsLive = 1 AND Kind = 'slip'
                  AND Ballot IN ('ward','pr') AND (@All = 1 OR Status = 'confirmed')
                GROUP BY Ballot, Party_name", con))
            {
                cmd.Parameters.AddWithValue("@E", election);
                cmd.Parameters.AddWithValue("@M", municipality);
                cmd.Parameters.AddWithValue("@All", all ? 1 : 0);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        partyRows.Add((dr["Ballot"].ToString() ?? "", NormalisedPartyKey(dr["Party_name"].ToString()),
                            dr["Name"].ToString() ?? "", Convert.ToInt64(dr["Votes"])));
                    }
                }
            }
        }

        //6 - ward ballot by ward and party (query 2 of the plan); leader/runner-up and
        //wards won are worked out in code below, from the normalised party key
        var wardRows = new List<(string Ward, string Key, string Name, long Votes, int VdsWithVotes)>();
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(@"
                SELECT Ward, Party_name, MAX(Candidate_name) AS Name, SUM(Votes) AS Votes, COUNT(DISTINCT VD) AS Vds
                FROM ResultsSlip
                WHERE Election = @E AND Municipality = @M AND IsLive = 1 AND Kind = 'slip' AND Ballot = 'ward'
                  AND (@All = 1 OR Status = 'confirmed')
                GROUP BY Ward, Party_name", con))
            {
                cmd.Parameters.AddWithValue("@E", election);
                cmd.Parameters.AddWithValue("@M", municipality);
                cmd.Parameters.AddWithValue("@All", all ? 1 : 0);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        wardRows.Add((dr["Ward"].ToString() ?? "", NormalisedPartyKey(dr["Party_name"].ToString()),
                            dr["Name"].ToString() ?? "", Convert.ToInt64(dr["Votes"]), Convert.ToInt32(dr["Vds"])));
                    }
                }
            }
        }

        //wards: total VDs per ward from the roll, counted VDs per ward from the ward
        //ballot's own coverage (not the combined ward+PR "counted" used for the strip)
        var vdsPerWard = registeredByVd.Values.GroupBy(v => v.Ward).ToDictionary(g => g.Key, g => g.Count());
        var countedVdsPerWard = registeredByVd
            .Where(kv => statusByVdBallot.TryGetValue((kv.Key, "ward"), out var s) && Counted(s))
            .GroupBy(kv => kv.Value.Ward)
            .ToDictionary(g => g.Key, g => g.Count());

        var wards = new JsonArray();
        var wardsWonByParty = new Dictionary<string, int>();
        var wardsLeadingByParty = new Dictionary<string, int>();
        foreach (var wardGroup in wardRows.GroupBy(w => w.Ward).OrderBy(g => g.Key))
        {
            var ranked = wardGroup.OrderByDescending(w => w.Votes).ToList();
            var leader = ranked.FirstOrDefault();
            var runnerUp = ranked.Skip(1).FirstOrDefault();
            var totalVds = vdsPerWard.GetValueOrDefault(wardGroup.Key, 0);
            var countedVds = countedVdsPerWard.GetValueOrDefault(wardGroup.Key, 0);
            var complete = totalVds > 0 && countedVds == totalVds;
            if (leader.Key != null)
            {
                if (complete)
                {
                    wardsWonByParty[leader.Key] = wardsWonByParty.GetValueOrDefault(leader.Key) + 1;
                }
                else
                {
                    wardsLeadingByParty[leader.Key] = wardsLeadingByParty.GetValueOrDefault(leader.Key) + 1;
                }
            }
            wards.Add(new JsonObject
            {
                ["ward"] = wardGroup.Key,
                ["vds"] = totalVds,
                ["vdsCounted"] = countedVds,
                ["leader"] = leader.Key == null ? null : new JsonObject
                {
                    ["party"] = leader.Key, ["name"] = leader.Name, ["votes"] = leader.Votes,
                },
                ["runnerUp"] = runnerUp.Key == null ? null : new JsonObject
                {
                    ["party"] = runnerUp.Key, ["votes"] = runnerUp.Votes,
                },
                ["complete"] = complete,
            });
        }

        //parties: ward + PR votes merged by normalised key. Only the PR row's own
        //"name" is trustworthy as the party's name here — a PR slip lists just the
        //party, no candidate. A ward slip's "name" is really the candidate printed
        //for that party in one specific ward, so it's never used even as a fallback;
        //without a PR row yet, ["name"] below falls back to the party key itself
        //(e.g. "ANC") instead of showing one ward candidate's name as if it were the
        //whole party.
        var partyTotals = new Dictionary<string, (string Name, long WardVotes, long PrVotes)>();
        foreach (var row in partyRows)
        {
            var (name, wardVotes, prVotes) = partyTotals.TryGetValue(row.Key, out var existing)
                ? existing : ("", 0L, 0L);
            if (row.Ballot == "pr") name = row.Name;
            partyTotals[row.Key] = row.Ballot == "ward"
                ? (name, wardVotes + row.Votes, prVotes)
                : (name, wardVotes, prVotes + row.Votes);
        }
        var parties = new JsonArray();
        foreach (var (key, info) in partyTotals.OrderByDescending(kv => kv.Value.WardVotes + kv.Value.PrVotes))
        {
            parties.Add(new JsonObject
            {
                ["key"] = key,
                ["name"] = string.IsNullOrEmpty(info.Name) ? key : info.Name,
                ["wardVotes"] = info.WardVotes,
                ["prVotes"] = info.PrVotes,
                ["totalVotes"] = info.WardVotes + info.PrVotes,
                ["wardsWon"] = wardsWonByParty.GetValueOrDefault(key),
                ["wardsLeading"] = wardsLeadingByParty.GetValueOrDefault(key),
            });
        }

        //7 - seat estimate, only when someone has entered this municipality's council
        //size (CouncilSeats); otherwise the dashboard just shows votes, as the plan says
        JsonObject? seats = null;
        var councilSeats = GetCouncilSeats(municipality);
        if (councilSeats != null)
        {
            var inputs = partyTotals.Select(kv => new SeatEstimator.PartyInput(
                kv.Key, (int)(kv.Value.WardVotes + kv.Value.PrVotes), (int)kv.Value.PrVotes,
                wardsWonByParty.GetValueOrDefault(kv.Key)));
            var estimate = SeatEstimator.Estimate_(councilSeats.Value.Seats, councilSeats.Value.IndependentWards, inputs);
            seats = new JsonObject
            {
                ["councilSeats"] = councilSeats.Value.Seats,
                ["independentWards"] = councilSeats.Value.IndependentWards,
                ["quota"] = estimate.Quota,
                ["parties"] = new JsonArray(estimate.Parties.Select(p => (JsonNode)new JsonObject
                {
                    ["key"] = p.Key, ["seats"] = p.Seats, ["wardSeats"] = p.WardSeats, ["prSeats"] = p.PrSeats,
                }).ToArray()),
            };
        }

        return new JsonObject
        {
            ["election"] = election,
            ["municipality"] = municipality,
            ["mode"] = all ? "all" : "confirmed",
            ["generatedAt"] = DateTime.UtcNow.ToString("o"),
            ["cacheSeconds"] = 60,
            ["coverage"] = new JsonObject
            {
                ["vds"] = vds,
                ["vdsWard"] = vdsWard,
                ["vdsPr"] = vdsPr,
                ["vdsBoth"] = vdsBoth,
                ["registered"] = registered,
                ["registeredCounted"] = registeredCounted,
                ["unconfirmedSlips"] = unconfirmedSlips,
                ["flaggedSlips"] = pagesMissing + illegible + unknownBallot + replaced,
            },
            ["parties"] = parties,
            ["wards"] = wards,
            ["seats"] = seats,
            ["flags"] = new JsonObject
            {
                ["pagesMissing"] = pagesMissing,
                ["illegible"] = illegible,
                ["unknownBallot"] = unknownBallot,
                ["replaced"] = replaced,
            },
        };
    }

    //GET /ResultsSlip/coverage/{municipality}: one row per VD with each ballot's status
    //and, unlike /summary's flag counts, the specific reason(s) that VD+ballot needs
    //attention - this is what actually lets the dashboard's VD grid and needs-attention
    //list point at a VD instead of just saying "9 flagged slips" with nowhere to click.
    private JsonArray BuildCoverage(string election, string municipality, IMemoryCache cache)
    {
        var registeredByVd = GetVdRoster(municipality, cache);

        var statusByVdBallot = new Dictionary<(string Vd, string Ballot), (bool Confirmed, bool Unconfirmed, bool Replaced)>();
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(@"
                SELECT VD, Ballot,
                       MAX(Status = 'confirmed' AND IsLive = 1) AS Confirmed,
                       MAX(Status = 'auto' AND IsLive = 1) AS Unconfirmed,
                       MAX(ReplacedAt IS NOT NULL) AS Replaced
                FROM ResultsSlip
                WHERE Election = @E AND Municipality = @M AND Kind = 'slip' AND Ballot IS NOT NULL
                GROUP BY VD, Ballot", con))
            {
                cmd.Parameters.AddWithValue("@E", election);
                cmd.Parameters.AddWithValue("@M", municipality);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        statusByVdBallot[(dr["VD"].ToString() ?? "", dr["Ballot"].ToString() ?? "")] =
                            (Convert.ToInt32(dr["Confirmed"]) > 0, Convert.ToInt32(dr["Unconfirmed"]) > 0,
                             Convert.ToInt32(dr["Replaced"]) > 0);
                    }
                }
            }
        }

        var reasonsByVdBallot = new Dictionary<(string Vd, string Ballot), List<string>>();
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand(@"
                SELECT VD, Ballot, MAX(PageCount) AS PageCount,
                       COUNT(DISTINCT CASE WHEN PageNo > 0 THEN PageNo END) AS Seen,
                       MAX(CASE WHEN Reading IS NOT NULL
                            AND JSON_CONTAINS(JSON_EXTRACT(Reading, '$.parties[*].legible'), 'false')
                            THEN 1 ELSE 0 END) AS Illegible
                FROM ResultsSlipImages
                WHERE Election = @E AND VD IN (SELECT VDNumber FROM AllVoters WHERE Municipality = @M)
                  AND Status IN ('read', 'saved')
                GROUP BY VD, Ballot", con))
            {
                cmd.Parameters.AddWithValue("@E", election);
                cmd.Parameters.AddWithValue("@M", municipality);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var vd = dr["VD"].ToString() ?? "";
                        var ballot = dr["Ballot"].ToString() ?? "";
                        var reasons = reasonsByVdBallot.TryGetValue((vd, ballot), out var existing) ? existing : [];
                        var pageCount = Convert.ToInt32(dr["PageCount"]);
                        var seen = Convert.ToInt32(dr["Seen"]);
                        if (pageCount > 0 && seen < pageCount)
                        {
                            reasons.Add($"page {seen + 1} of {pageCount} missing");
                        }
                        if (Convert.ToInt32(dr["Illegible"]) > 0)
                        {
                            reasons.Add("numbers illegible");
                        }
                        if (ballot == "unknown")
                        {
                            reasons.Add("ballot type not recognised");
                        }
                        reasonsByVdBallot[(vd, ballot)] = reasons;
                    }
                }
            }
        }

        var rows = new JsonArray();
        foreach (var (vd, info) in registeredByVd.OrderBy(kv => kv.Key))
        {
            var ward = info.Ward;
            var ballots = new JsonObject();
            // Always show ward/pr as the two columns the VD grid expects, but also pick
            // up any other ballot value this VD actually has data for - chiefly
            // "unknown" (a slip whose ballot type the reader couldn't tell), which
            // otherwise has a real reason recorded above (reasonsByVdBallot) that never
            // reached the response: a VD with only an unknown-ballot slip showed no
            // flag at all here, even though /summary's aggregate counts it.
            var ballotKeys = new HashSet<string> { "ward", "pr" };
            foreach (var key in statusByVdBallot.Keys.Where(k => k.Vd == vd).Select(k => k.Ballot)) ballotKeys.Add(key);
            foreach (var key in reasonsByVdBallot.Keys.Where(k => k.Vd == vd).Select(k => k.Ballot)) ballotKeys.Add(key);
            foreach (var ballot in ballotKeys)
            {
                statusByVdBallot.TryGetValue((vd, ballot), out var status);
                var reasons = new List<string>(reasonsByVdBallot.GetValueOrDefault((vd, ballot), []));
                if (status.Replaced)
                {
                    reasons.Add("replaced by a second user");
                }
                var state = status.Confirmed ? "confirmed" : status.Unconfirmed ? "auto" : "missing";
                ballots[ballot] = new JsonObject
                {
                    ["status"] = state,
                    ["reasons"] = new JsonArray(reasons.Select(r => (JsonNode)JsonValue.Create(r)!).ToArray()),
                };
            }
            rows.Add(new JsonObject
            {
                ["vd"] = vd,
                ["ward"] = ward,
                ["ballots"] = ballots,
            });
        }
        return rows;
    }

    private int MaxScansPerDay() =>
        int.TryParse(_config?["ResultsSlip:MaxScansPerDay"], out var max) ? max : 60;

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
    //all pages of a multi-page slip
    public List<long>? SlipIds { get; set; }
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
