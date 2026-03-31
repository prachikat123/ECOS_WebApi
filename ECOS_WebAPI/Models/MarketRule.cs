namespace ECOS_WebAPI.Models
{
    public class MarketRule
    {
        public string RuleName { get; set; }
        public bool IsPassed { get; set; }
        public string Reason { get; set; }
    }
}
