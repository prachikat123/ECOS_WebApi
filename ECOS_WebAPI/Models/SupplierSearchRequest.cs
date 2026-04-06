namespace ECOS_WebAPI.Models
{
    public class SupplierSearchRequest
    {
        public string ProductName { get; set; }
        public string Country { get; set; }
        public int TargetMOQ { get; set; }

        public decimal TargetSellingPrice { get; set; }
    }
}
