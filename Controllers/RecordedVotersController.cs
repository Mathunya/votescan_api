using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Web_Api.Models;

namespace Web_Api.Controllers;

[Route("[controller]")]
[ApiController]
public class RecordedVotersController : ControllerBase
{
    private IConfiguration _config;
    private RecordedVoters? v;
    private string? connect;
    private readonly IHttpClientFactory _httpClientFactory;

    public RecordedVotersController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _config = configuration;
        connect = _config.GetConnectionString("ConsString");
        v = new RecordedVoters();
        _httpClientFactory = httpClientFactory;
    }
    //GET ALL VOTERS
    [HttpGet]
    public IEnumerable<RecordedVoters> getall()
    {
        MySqlDataReader dr;
        List<RecordedVoters> all = new List<RecordedVoters>();
        //setup database cons

        //initiate connection
        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@selector", "all");
                //open reader
                AddMissingGetRecordedVotersParameters(cmd);
                cmd.AddMissingStoredProcedureParameters();
                dr = cmd.ExecuteReader();
                //fill data in datatable
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
                    v.voting_DISTRICT = dr["voting_DISTRICT"].ToString();
                    v.issuesArr = dr["issuesArr"].ToString();
                    v.status = dr["status"].ToString();
                    v.date = dr["date"].ToString();
                    v.cell = dr["cell"].ToString();
                    v.attitude = dr["attitude"].ToString();
                    v.volunteer = dr["volunteer"].ToString();
                    v.status = dr["status"].ToString();
                    v.transport = dr["transport"].ToString();
                    v.special = dr["special"].ToString();
                    v.date = dr["date"].ToString();
                    all.Add(v);
                }
                //close connections
                dr.Close();
                con.Close();
            }
        }
        return all.ToArray();
    }
    //GET ALL VOTERS BY ID NUMBER
    [HttpGet]
    [Route("getbycell/{cell}")]
    public IEnumerable<RecordedVoters> getbycell(string cell)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cell", cell);
                cmd.Parameters.AddWithValue("@selector", "getbycell");

                AddMissingGetRecordedVotersParameters(cmd);

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
                        v.voting_DISTRICT = dr["voting_DISTRICT"].ToString();
                        v.issuesArr = dr["issuesArr"].ToString();
                        v.status = dr["status"].ToString();
                        v.date = dr["date"].ToString();
                        v.cell = dr["cell"].ToString();
                        v.attitude = dr["attitude"].ToString();
                        v.volunteer = dr["volunteer"].ToString();
                        v.status = dr["status"].ToString();
                        v.transport = dr["transport"].ToString();
                        v.special = dr["special"].ToString();
                        v.date = dr["date"].ToString();
                        v.election = dr["election"].ToString();
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
        //GET ALL VOTERS BY ID NUMBER
    [HttpGet]
    [Route("getbyidandsurname/{id}/{surname}")]
    public IEnumerable<RecordedVoters> getbyidandsurname(string id,string surname)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_Number", id);
                cmd.Parameters.AddWithValue("@surname", surname);
                cmd.Parameters.AddWithValue("@selector", "getbyidandsurname");

                AddMissingGetRecordedVotersParameters(cmd);

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
                        v.voting_DISTRICT = dr["voting_DISTRICT"].ToString();
                        v.issuesArr = dr["issuesArr"].ToString();
                        v.status = dr["status"].ToString();
                        v.date = dr["date"].ToString();
                        v.cell = dr["cell"].ToString();
                        v.attitude = dr["attitude"].ToString();
                        v.volunteer = dr["volunteer"].ToString();
                        v.status = dr["status"].ToString();
                        v.transport = dr["transport"].ToString();
                        v.special = dr["special"].ToString();
                        v.date = dr["date"].ToString();
                        v.election = dr["election"].ToString();
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
    [Route("getbyvolunteer/{cell}")]
    public IEnumerable<RecordedVoters> getbyvolunteer(string cell)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@volunteer", cell);
                cmd.Parameters.AddWithValue("@selector", "getbyvolunteer");

                AddMissingGetRecordedVotersParameters(cmd);

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
                        v.voting_DISTRICT = dr["voting_DISTRICT"].ToString();
                        v.issuesArr = dr["issuesArr"].ToString();
                        v.status = dr["status"].ToString();
                        v.date = dr["date"].ToString();
                        v.cell = dr["cell"].ToString();
                        v.attitude = dr["attitude"].ToString();
                        v.volunteer = dr["volunteer"].ToString();
                        v.transport = dr["transport"].ToString();
                        v.special = dr["special"].ToString();
                        v.status = dr["status"].ToString();
                        v.date = dr["date"].ToString();
                        v.election = dr["election"].ToString();
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
    [Route("getbyward/{ward}")]
    public IEnumerable<RecordedVoters> getbyward(string ward)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ward", ward);
                cmd.Parameters.AddWithValue("@selector", "getbyward");

                AddMissingGetRecordedVotersParameters(cmd);

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
                        v.voting_DISTRICT = dr["voting_DISTRICT"].ToString();
                        v.issuesArr = dr["issuesArr"].ToString();
                        v.status = dr["status"].ToString();
                        v.date = dr["date"].ToString();
                        v.cell = dr["cell"].ToString();
                        v.attitude = dr["attitude"].ToString();
                        v.volunteer = dr["volunteer"].ToString();
                        v.transport = dr["transport"].ToString();
                        v.special = dr["special"].ToString();
                        v.status = dr["status"].ToString();
                        v.date = dr["date"].ToString();
                        v.election = dr["election"].ToString();
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
    [Route("getspecial/{vs}")]
    public IEnumerable<RecordedVoters> getspecial(string vs)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@voting_Station", vs);
                cmd.Parameters.AddWithValue("@selector", "getspecial");

                AddMissingGetRecordedVotersParameters(cmd);

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
                        v.voting_DISTRICT = dr["voting_DISTRICT"].ToString();
                        v.issuesArr = dr["issuesArr"].ToString();
                        v.status = dr["status"].ToString();
                        v.date = dr["date"].ToString();
                        v.cell = dr["cell"].ToString();
                        v.attitude = dr["attitude"].ToString();
                        v.volunteer = dr["volunteer"].ToString();
                        v.transport = dr["transport"].ToString();
                        v.special = dr["special"].ToString();
                        v.status = dr["status"].ToString();
                        v.date = dr["date"].ToString();
                        v.election = dr["election"].ToString();
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
    [Route("gettransport/{vs}")]
    public IEnumerable<RecordedVoters> gettransport(string vs)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@voting_Station", vs);
                cmd.Parameters.AddWithValue("@selector", "gettransport");

                AddMissingGetRecordedVotersParameters(cmd);

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
                        v.voting_DISTRICT = dr["voting_DISTRICT"].ToString();
                        v.issuesArr = dr["issuesArr"].ToString();
                        v.status = dr["status"].ToString();
                        v.date = dr["date"].ToString();
                        v.cell = dr["cell"].ToString();
                        v.attitude = dr["attitude"].ToString();
                        v.volunteer = dr["volunteer"].ToString();
                        v.transport = dr["transport"].ToString();
                        v.special = dr["special"].ToString();
                        v.status = dr["status"].ToString();
                        v.date = dr["date"].ToString();
                        v.election = dr["election"].ToString();
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
    [Route("getwardprogress/{ward}/{startdate}/{electiondate}")]
    public IEnumerable<RecordedVoters> getwardprogress(string ward,string startdate,string electiondate)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ward", ward);
                cmd.Parameters.AddWithValue("@start_date",  Convert.ToDateTime(startdate));
                cmd.Parameters.AddWithValue("@date",  Convert.ToDateTime(electiondate));
                cmd.Parameters.AddWithValue("@selector", "wardprogress");

                AddMissingGetRecordedVotersParameters(cmd);

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
                    //close connections
                    dr.Close();
                    con.Close();
                }
            }
        }
        return all.ToArray();
    }
    [HttpGet]
    [Route("getprovinceprogress/{election}/{startdate}/{date}")]
    public IEnumerable<RecordedVoters> getprovinceprogress(string election,string startdate, string date)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@election", election);
                cmd.Parameters.AddWithValue("@start_date", Convert.ToDateTime(startdate));
                cmd.Parameters.AddWithValue("@date", Convert.ToDateTime(date));
                cmd.Parameters.AddWithValue("@selector", "getprovinceprogress");

                AddMissingGetRecordedVotersParameters(cmd);

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
                    //close connections
                    dr.Close();
                    con.Close();
                }
            }
        }
        return all.ToArray();
    }
    [HttpGet]
    [Route("getmunicipalityprogress/{election}/{startdate}/{date}")]
    public IEnumerable<RecordedVoters> getmunicipalityprogress(string election,string startdate,string date)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@election", election);
                cmd.Parameters.AddWithValue("@start_date", Convert.ToDateTime(startdate));
                cmd.Parameters.AddWithValue("@date", Convert.ToDateTime(date));
                cmd.Parameters.AddWithValue("@selector", "getmunicipalityprogress");

                AddMissingGetRecordedVotersParameters(cmd);

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
    public IEnumerable<RecordedVoters> getwardlistprogress(string municipality)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@municipality", municipality);
                cmd.Parameters.AddWithValue("@selector", "getwardlistprogress");

                AddMissingGetRecordedVotersParameters(cmd);

                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.ward = dr["ward"].ToString();//takes municipality
                        v.id = dr["scanned"].ToString();//takes scanned

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
    [Route("countcontact/{id}/{surname}")]
    public IEnumerable<RecordedVoters> countcontact(string id,string surname)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_number", id);
                cmd.Parameters.AddWithValue("@surname", surname);
                cmd.Parameters.AddWithValue("@selector", "countcontact");

                AddMissingGetRecordedVotersParameters(cmd);

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
                    //close connections
                    dr.Close();
                    con.Close();
                }
            }
        }
        return all.ToArray();
    }
    [HttpGet]
    [Route("vdnumbers/{ward}/{startdate}/{electiondate}")]
    public IEnumerable<RecordedVoters> vdnumbers(string ward,string startdate,string electiondate)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ward", ward);
                cmd.Parameters.AddWithValue("@start_date", Convert.ToDateTime(startdate));
                cmd.Parameters.AddWithValue("@date",Convert.ToDateTime(electiondate));
                cmd.Parameters.AddWithValue("@selector", "vdnumbers");

                AddMissingGetRecordedVotersParameters(cmd);

                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.voting_DISTRICT = dr["VDNumber"].ToString();//vd number
                        v.voting_station = dr["VDName"].ToString();//vd number
                        v.id = dr["Total"].ToString();//takes total recorded per vd
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
    public async Task<int> addRecorded(RecordedVoters vdata)
    {
        int retCode = 0;

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@selector", "add");
                //
                cmd.Parameters.AddWithValue("@surname", vdata.surname);
                cmd.Parameters.AddWithValue("@names", vdata.names);
                cmd.Parameters.AddWithValue("@id_number", vdata.id_number);
                cmd.Parameters.AddWithValue("@address", vdata.address);
                cmd.Parameters.AddWithValue("@province", vdata.province);
                cmd.Parameters.AddWithValue("@municipality", vdata.municipality);
                cmd.Parameters.AddWithValue("@voting_station", vdata.voting_station);
                cmd.Parameters.AddWithValue("@ward", vdata.ward);
                cmd.Parameters.AddWithValue("@voting_DISTRICT", vdata.voting_DISTRICT);

                cmd.Parameters.AddWithValue("@cell", vdata.cell);
                cmd.Parameters.AddWithValue("@attitude", vdata.attitude);
                cmd.Parameters.AddWithValue("@issuesArr", vdata.issuesArr);
                cmd.Parameters.AddWithValue("@special", vdata.special);
                cmd.Parameters.AddWithValue("@transport", vdata.transport);
                cmd.Parameters.AddWithValue("@volunteer", vdata.volunteer);

                cmd.Parameters.AddWithValue("@election", vdata.election);
                cmd.Parameters.AddWithValue("@gender", vdata.gender);
                AddMissingGetRecordedVotersParameters(cmd);
                con.Open();
                cmd.AddMissingStoredProcedureParameters();
                var ret = cmd.ExecuteNonQuery();
                retCode = Convert.ToInt32(ret);
                con.Close();
            }
        }

        if (retCode > 0)
        {
            // A WhatsApp delivery problem should never fail the voter record
            // save itself, so this is fire-and-forget from the caller's
            // perspective — failures are swallowed and logged inside.
            await SendWhatsAppConfirmationAsync(vdata.cell, vdata.names);
        }

        //  -1 =failue
        //   1 = account saved successfully
        return retCode;
    }

    // Sends a WhatsApp template confirmation to the voter via the Meta
    // WhatsApp Cloud API. Requires WhatsApp:PhoneNumberId, WhatsApp:AccessToken
    // and an approved WhatsApp:TemplateName to be configured — until then this
    // is a silent no-op so voter capture keeps working without WhatsApp set up.
    private async Task SendWhatsAppConfirmationAsync(string? cell, string? name)
    {
        var phoneNumberId = _config["WhatsApp:PhoneNumberId"];
        var accessToken = _config["WhatsApp:AccessToken"];
        var templateName = _config["WhatsApp:TemplateName"];
        var templateLanguage = _config["WhatsApp:TemplateLanguage"] ?? "en_US";
        var apiVersion = _config["WhatsApp:ApiVersion"] ?? "v21.0";
        var headerImageMediaId = _config["WhatsApp:HeaderImageMediaId"];

        if (string.IsNullOrWhiteSpace(phoneNumberId) ||
            string.IsNullOrWhiteSpace(accessToken) ||
            string.IsNullOrWhiteSpace(templateName))
        {
            return;
        }

        var toNumber = FormatSouthAfricanNumber(cell);
        if (toNumber == null)
        {
            return;
        }

        var components = new List<object>();
        // votescan_contact_history's approved header is an image — send the
        // app logo, referenced by a media ID uploaded once via the Media API
        // (POST /{phone-number-id}/media), rather than re-uploading per send.
        if (!string.IsNullOrWhiteSpace(headerImageMediaId))
        {
            components.Add(new
            {
                type = "header",
                parameters = new object[]
                {
                    new { type = "image", image = new { id = headerImageMediaId } }
                }
            });
        }
        components.Add(new
        {
            type = "body",
            parameters = new object[]
            {
                new { type = "text", text = string.IsNullOrWhiteSpace(name) ? "there" : name }
            }
        });

        var payload = new
        {
            messaging_product = "whatsapp",
            to = toNumber,
            type = "template",
            template = new
            {
                name = templateName,
                language = new { code = templateLanguage },
                components = components,
            }
        };

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"https://graph.facebook.com/{apiVersion}/{phoneNumberId}/messages")
            {
                Content = JsonContent.Create(payload),
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"WhatsApp send failed ({(int)response.StatusCode}): {body}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WhatsApp send error: {ex.Message}");
        }
    }

    // Converts a locally-captured SA cell number (e.g. "0821234567") into the
    // E.164 digits-only format the WhatsApp Cloud API expects (e.g. "27821234567").
    private static string? FormatSouthAfricanNumber(string? cell)
    {
        if (string.IsNullOrWhiteSpace(cell))
        {
            return null;
        }

        var digits = new string(cell.Where(char.IsDigit).ToArray());

        if (digits.Length == 10 && digits.StartsWith("0"))
        {
            return "27" + digits.Substring(1);
        }
        if (digits.Length == 11 && digits.StartsWith("27"))
        {
            return digits;
        }
        if (digits.Length == 9)
        {
            return "27" + digits;
        }

        return null;
    }
    [HttpGet]
    [Route("gettop10")]
    public IEnumerable<RecordedVoters> gettop10()
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@selector", "gettop10");

                AddMissingGetRecordedVotersParameters(cmd);

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
                        v.voting_DISTRICT = dr["voting_DISTRICT"].ToString();
                        v.issuesArr = dr["issuesArr"].ToString();
                        v.status = dr["status"].ToString();
                        v.date = dr["date"].ToString();
                        v.cell = dr["cell"].ToString();
                        v.attitude = dr["attitude"].ToString();
                        v.volunteer = dr["volunteer"].ToString();
                        v.transport = dr["transport"].ToString();
                        v.special = dr["special"].ToString();
                        v.status = dr["status"].ToString();
                        v.date = dr["date"].ToString();
                        v.election = dr["election"].ToString();
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
    [Route("getcanvassedtotal")]
    public IEnumerable<RecordedVoters> getcanvassedtotal()
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@selector", "getcanvassedtotal");

                AddMissingGetRecordedVotersParameters(cmd);

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
                    //close connections
                    dr.Close();
                    con.Close();
                }
            }
        }
        return all.ToArray();
    }
    // GET CANVASSING TOTALS OVER A TIMEFRAME, SCOPED BY WHAT'S SENT AS "municipality":
    //   "all"                                  -> every municipality in Free State
    //   a district region (e.g. "Fezile Dabi") -> municipalities within that region
    //   a municipality name (e.g. "Matjhabeng") -> wards within that municipality
    //   a Mangaung zone (e.g. "zone 1")         -> wards within that zone
    //   a Matjhabeng/Maluti a Phofung cluster    -> wards within that cluster
    //     (e.g. "Thabong Central", "Monontsha")
    // timeframe: today | week | 2weeks | 30days | 90days
    [HttpGet]
    [Route("getbytimeframe/{municipality}/{timeframe}")]
    public IEnumerable<RecordedVoters> getbytimeframe(string municipality, string timeframe)
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@municipality", municipality);
                cmd.Parameters.AddWithValue("@timeframe", timeframe);
                cmd.Parameters.AddWithValue("@selector", "getbytimeframe");

                AddMissingGetRecordedVotersParameters(cmd);

                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.province = dr["province"].ToString();
                        v.municipality = dr["municipality"].ToString();
                        v.ward = dr["ward"].ToString();
                        v.id = dr["canvassed"].ToString();
                        v.date = dr["range_start"].ToString();
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
    [Route("getwardlist")]
    public IEnumerable<RecordedVoters> getwardlist()
    {
        List<RecordedVoters> all = new List<RecordedVoters>();

        using (MySqlConnection con = new MySqlConnection(connect))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@selector", "getwardlist");

                AddMissingGetRecordedVotersParameters(cmd);

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
                    //close connections
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
            using (MySqlCommand cmd = new MySqlCommand("getRecordedVoters", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@selector", "getcellnumbers");

                AddMissingGetRecordedVotersParameters(cmd);

                cmd.AddMissingStoredProcedureParameters();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        v = new RecordedVoters();
                        v.cell = dr["cell"].ToString();
                    
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

    private static void AddMissingGetRecordedVotersParameters(MySqlCommand cmd)
    {
        string[] parameterNames =
        {
            "@surname",
            "@names",
            "@id_Number",
            "@address",
            "@province",
            "@municipality",
            "@voting_Station",
            "@ward",
            "@voting_DISTRICT",
            "@cell",
            "@attitude",
            "@issuesArr",
            "@special",
            "@transport",
            "@volunteer",
            "@election",
            "@gender",
            "@start_date",
            "@date",
            "@timeframe",
            "@selector"
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
