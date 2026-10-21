namespace Web_Api.Models
{
    // One row of Reports/getdailycanvassing (voters canvassed in one VD on one
    // day) or Reports/getvdroll (one VD on the roll, with Name set).
    public class DailyCanvassing
    {
        public string? Name
        {
            get; set;
        }
        public string? Municipality
        {
            get; set;
        }
        public string? Ward
        {
            get; set;
        }
        public string? Vd
        {
            get; set;
        }
        public string? Day
        {
            get; set;
        }
        public int Recorded
        {
            get; set;
        }
    }
}
