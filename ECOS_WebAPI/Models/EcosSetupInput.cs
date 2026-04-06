namespace ECOS_WebAPI.Models
{
    public class EcosSetupInput
    {
        public string UserId { get; set; }
        // 1. Platform Connections
        public bool ShopifyConnected { get; set; }
        public bool MetaConnected { get; set; }
        public bool TikTokConnected { get; set; }
        public bool GoogleAdsConnected { get; set; }
        public bool KlaviyoConnected { get; set; }
        public bool SupplierApiConnected { get; set; }
        public bool GTMConnected { get; set; }

        // 2. Market
        public string TargetCountry { get; set; }
        public string Language { get; set; }
        public string Currency { get; set; }


        // 3. Preferences
        public string Niche { get; set; }
        public string WebsiteUrl { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal MinMargin { get; set; }
        public string CustomerAvatar { get; set; }

        public int ExpectedDailyOrders { get; set; }

        // 4. Exclusions
        public List<string> ExcludedCategories { get; set; }
        public List<string> ExcludedKeywords { get; set; }
    }
}
