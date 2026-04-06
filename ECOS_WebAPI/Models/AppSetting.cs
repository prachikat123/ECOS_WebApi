namespace ECOS_WebAPI.Models
{
    public class AppSetting
    {
        public ShopifySettings Shopify { get; set; }

        public MetaSettings Meta { get; set; }

        public OpenRouterSettings OpenRouter { get; set; }
    }
}
