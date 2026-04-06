using System.Text.Json.Serialization;

namespace ECOS_WebAPI.Models.Shopify
{
    public class ShopifyProductRequest
    {
        //  Product Basic Info
        public string Title { get; set; }
        public string Description { get; set; }
        public string Vendor { get; set; }  // brand/store name
        public string ProductType { get; set; }

        //  Pricing
        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; } // MRP / discount logic

        //  Inventory
        public int? StockQuantity { get; set; }
        public string SKU { get; set; }

        //  Images (URLs or uploaded links)
        public List<string> Images { get; set; }

        //  SEO / Tags
        public List<string> Tags { get; set; }

        // Shopify settings
        public bool IsPublished { get; set; } = true;

    }
}
