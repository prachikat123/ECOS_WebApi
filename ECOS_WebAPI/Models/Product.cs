namespace ECOS_WebAPI.Models
{
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public decimal EstimatedMargin { get; set; }
        public bool IsApproved {  get; set; }

    }
}
