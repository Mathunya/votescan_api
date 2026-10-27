using System.Text.Json;
using Anthropic;
using Anthropic.Models.Beta;
using Anthropic.Models.Beta.Messages;

namespace Web_Api.Services;

// Reads a photographed IEC results slip with Claude vision and returns the VD
// and each party's handwritten vote count as JSON.
public class ResultsSlipReader
{
    private const string Model = "claude-opus-5-5";

    private const string Instructions = """
        You are transcribing a photographed South African IEC election results slip.

        The slip has a printed heading (province, municipality, "VD" followed by an 8 digit
        voting district number, the voting station name, registered voters) and a table of
        parties. Each table row has a printed party name, a printed abbreviation, and a
        handwritten number in the Result column.

        Transcribe:
        - vd_number: the 8 digit number printed after "VD". vd_name: the voting station name below it.
        - province, municipality, registered_voters and the ballot/election title as printed.
        - page: the "Page x of y" text if present.
        - parties: one entry per table row, top to bottom in the printed order. Copy the printed
          name and abbreviation exactly. votes is the handwritten Result for that row as a whole
          number; an empty Result cell is 0.
        - legible: false when you are not sure of a handwritten number (smudged, overwritten,
          cut off, or ambiguous digits such as 1/7 or 0/6). Still give your best reading in votes.

        Only report what is on the slip. Never add parties that are not printed. Leave a text
        field empty when it is not on the slip. Use notes for anything a person checking the
        slip should know, such as a cut-off photo or corrections on the slip.
        """;

    private readonly AnthropicClient _client;

    public ResultsSlipReader(IConfiguration configuration)
    {
        // never shipped in the app - the key stays on the server
        _client = new AnthropicClient
        {
            ApiKey = configuration["Anthropic:ApiKey"]
                ?? Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY"),
        };
    }

    public async Task<JsonElement> ReadAsync(string base64Image, string mediaType)
    {
        var response = await _client.Beta.Messages.Create(new MessageCreateParams
        {
            Model = Model,
            MaxTokens = 16000,
            // re-run on another model if a safety check declines the slip
            Betas = [AnthropicBeta.ServerSideFallback2026_07_01],
            Fallbacks = new Default(),
            OutputConfig = new BetaOutputConfig
            {
                // handwritten digits need careful reading
                Effort = Effort.High,
                Format = new BetaJsonOutputFormat { Schema = SlipSchema },
            },
            Messages =
            [
                new BetaUserMessageParam
                {
                    Content = new List<BetaContentBlockParam>
                    {
                        new BetaImageBlockParam
                        {
                            Source = new BetaBase64ImageSource
                            {
                                Data = base64Image,
                                MediaType = mediaType,
                            },
                        },
                        new BetaTextBlockParam { Text = Instructions },
                    },
                },
            ],
        });

        if (response.StopReason == "refusal")
        {
            throw new InvalidOperationException("The slip could not be read. Please retake the photo.");
        }
        var text = string.Concat(response.Content
            .Select(block => block.Value)
            .OfType<BetaTextBlock>()
            .Select(block => block.Text));
        return JsonDocument.Parse(text).RootElement.Clone();
    }

    private static readonly Dictionary<string, JsonElement> SlipSchema = new()
    {
        ["type"] = JsonSerializer.SerializeToElement("object"),
        ["additionalProperties"] = JsonSerializer.SerializeToElement(false),
        ["required"] = JsonSerializer.SerializeToElement(new[]
        {
            "vd_number", "vd_name", "province", "municipality", "registered_voters",
            "election_title", "page", "parties", "notes",
        }),
        ["properties"] = JsonSerializer.SerializeToElement(new Dictionary<string, object>
        {
            ["vd_number"] = new { type = "string" },
            ["vd_name"] = new { type = "string" },
            ["province"] = new { type = "string" },
            ["municipality"] = new { type = "string" },
            ["registered_voters"] = new { type = "string" },
            ["election_title"] = new { type = "string" },
            ["page"] = new { type = "string" },
            ["notes"] = new { type = "string" },
            ["parties"] = new
            {
                type = "array",
                items = new
                {
                    type = "object",
                    additionalProperties = false,
                    required = new[] { "name", "abbreviation", "votes", "legible" },
                    properties = new
                    {
                        name = new { type = "string" },
                        abbreviation = new { type = "string" },
                        votes = new { type = "integer" },
                        legible = new { type = "boolean" },
                    },
                },
            },
        }),
    };
}
