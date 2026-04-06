namespace ECOS_WebAPI.Models
{
    public class Product
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? EstimatedMargin { get; set; }
        public bool IsApproved {  get; set; }
        public int RelevanceScore { get; set; }

        public List<string> Images { get; set; }

    }
}
