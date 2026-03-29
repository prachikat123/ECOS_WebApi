namespace ECOS_WebAPI.Models
{
    public class EvaluationModel
    {
        public string ProductName { get; set; }

        public int TrendScore { get; set; }
        public int DemandScore { get; set; }
        public int CompetitionScore { get; set; }
        public int ProfitScore { get; set; }
        public int ShippingScore { get; set; }
        public int AudienceScore { get; set; }
        public int RepeatScore { get; set; }

        public int TotalScore { get; set; }

        public string Recommendation { get; set; }

    }
}
