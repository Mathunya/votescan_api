namespace Web_Api.Models
{
    public class FrontDeskCount
    {
        public string? region { get; set; }
        public string? municipality { get; set; } // also holds WardID for wardcount, matching VoterReg.VoterRegCount's convention
        public string? vdName { get; set; }
        public string? vdNumber { get; set; }
        public int? total1 { get; set; } //todays total
        public int? total0 { get; set; } //yesterdays total
    }
}
