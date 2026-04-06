namespace ECOS_WebAPI.Models
{
    public class CostBreakdown
    {
        public decimal ProductCost { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal CustomsCost { get; set; }
        public decimal PackagingCost { get; set; }

        public decimal TotalLandedCost { get; set; }
    }
}
