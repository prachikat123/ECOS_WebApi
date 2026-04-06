namespace ECOS_WebAPI.Models
{
    public class Supplier
    {
        public string Name { get; set; }
        public string Platform { get; set; } 
        public decimal UnitPrice { get; set; }
        public int MOQ { get; set; }
        public int ShippingDays { get; set; }
        public decimal DefectRate { get; set; } 
        public bool SupportsBranding { get; set; }
    }
}
