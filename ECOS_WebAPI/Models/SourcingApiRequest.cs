namespace ECOS_WebAPI.Models
{
    public class SourcingApiRequest
    {
        public string ProductName { get; set; }

        public decimal TargetSellingPrice { get; set; }
        public int ExpectedDailyOrders { get; set; }
        public string Country { get; set; }
        public int TargetMOQ { get; set; }
    }
}
