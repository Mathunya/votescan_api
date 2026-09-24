using System.Text.Json.Serialization;

namespace Web_Api.Models
{
    public class User
    {
        public string? Id
        {
            get; set;
        }
        public string? Name
        {
            get; set;
        }
        public string? Surname
        {
            get; set;
        }
        // Omitted from responses when not set, so list endpoints never send a "password" key.
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Password
        {
            get; set;
        }
        public string? Cell
        {
            get; set;
        }
        public string? Ward
        {
            get; set;
        }

        public string? Voting_Station
        {
            get; set;
        }
        public string? Voting_district
        {
            get; set;
        }
        public string? Date
        {
            get; set;
        }
        public string? Role
        {
            get; set;
        }
        public string? Delegation
        {
            get; set;
        }
        public string? Province
        {
            get; set;
        }
        public string? Municipality
        {
            get; set;
        }
        public string? Region
        {
            get; set;
        }
        public string? Report
        {
            get; set;
        }
    }
}