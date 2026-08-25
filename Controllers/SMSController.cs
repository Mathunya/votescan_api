using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;

namespace Web_Api.Controllers;

[Route("[controller]")]
[ApiController]
public class SMSController : ControllerBase
{
    private IConfiguration? _config;
    private Elections? e;
    private string? connect;

    public SMSController(IConfiguration configuration)
    {
        _config = configuration;
        connect = _config.GetConnectionString("ConsString");
        e = new Elections();
    }
    
    [HttpGet]
    [Route("sendSms/{to}/{body}")]
    public int sendSms(string to, string body)
    {
        int retcode=0;
        string myURI = "https://api.bulksms.com/v1/messages";

        // change these values to match your own account
        string myUsername = "AMoroeng";
        string myPassword = "Mathunya@1";

        // the details of the message we want to send
        string myData = "{to: \""+to+"\", body:\""+body+"\"}";

        // build the request based on the supplied settings
        #pragma warning disable SYSLIB0014 // Type or member is obsolete
        var request = WebRequest.Create(myURI);
        #pragma warning restore SYSLIB0014 // Type or member is obsolete

        // supply the credentials
        request.Credentials = new NetworkCredential(myUsername, myPassword);
        request.PreAuthenticate = true;
        // we want to use HTTP POST
        request.Method = "POST";
        // for this API, the type must always be JSON
        request.ContentType = "application/json";

        // Here we use Unicode encoding, but ASCIIEncoding would also work
        var encoding = new UnicodeEncoding();
        var encodedData = encoding.GetBytes(myData);

        // Write the data to the request stream
        var stream = request.GetRequestStream();
        stream.Write(encodedData, 0, encodedData.Length);
        stream.Close();

        // try ... catch to handle errors nicely
        try
        {
            // make the call to the API
            var response = request.GetResponse();

            // read the response and print it to the console
            var reader = new StreamReader(response.GetResponseStream());
            retcode=1;
        }
        catch (WebException ex)
        {
            // print the detail that comes with the error
            var reader = new StreamReader(ex.Message);
            retcode=-1;
        }
        return retcode;
    }
    
}