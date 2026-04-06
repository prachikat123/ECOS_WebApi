namespace ECOS_WebAPI.Models
{
    public class SourcingResult
    {
        public Supplier Supplier { get; set; }
        public CostBreakdown Cost { get; set; }
        public decimal SuggestedSellingPrice { get; set; }
        public decimal MarginPercentage { get; set; }

        //public decimal DailyProfit { get; set; }
        //public object ScalingSimulation { get; set; }
        public List<string> RiskFlags { get; set; }

        public decimal Score { get; set; }

        public bool IsApproved { get; set; }

        public decimal DailyAdBudget { get; set; }
    }
}
