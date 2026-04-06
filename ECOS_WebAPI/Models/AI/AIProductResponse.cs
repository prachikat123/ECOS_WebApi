namespace ECOS_WebAPI.Models.AI
{
    public class AIProductResponse
    {
        public Product Product { get; set; }

        public string Reasoning { get; set; }
        public int OpportunityScore { get; set; }

        public bool IsValidForLaunch { get; set; }
        public List<string> Warnings { get; set; }

        //public decimal TotalEstimatedRevenue { get; set; }
    }
}
