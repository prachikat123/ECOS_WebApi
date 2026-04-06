namespace ECOS_WebAPI.Models
{
    public class SourcingSettings
    {
        public decimal TargetMarginPercentage { get; set; }
        public decimal DefaultShippingCostPerUnit { get; set; }
        public decimal CustomsPercentage { get; set; }
        public decimal PackagingCostPerUnit { get; set; }

        public decimal CostWeight { get; set; }
        public decimal ShippingWeight { get; set; }
        public decimal DefectWeight { get; set; }
        public decimal BrandingBonus { get; set; }
    }
}
