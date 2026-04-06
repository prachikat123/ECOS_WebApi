namespace ECOS_WebAPI.Models.Shopify
{
    public class ShopifyProductResponse
    {
        // Shopify product ID
        public string ProductId { get; set; }

        //  Public product page URL
        public string ProductUrl { get; set; }

        //  Variant info(Shopify uses variants always)
        public List<ProductVariant> Variants { get; set; }

        // First image URL (useful for ads)
        public string MainImageUrl { get; set; }

        // Status
        public string Status { get; set; } // active / draft

        // Shopify handle (SEO friendly slug)
        public string Handle { get; set; }
    }

    public class ProductVariant
    {
        public string VariantId { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public int InventoryQuantity { get; set; }
    }
}
