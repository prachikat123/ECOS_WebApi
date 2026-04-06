namespace ECOS_WebAPI.Rules
{
    public class ProductRelevanceRule
    {
        private readonly NicheKeywordProvider _keywordProvider;

        public ProductRelevanceRule(NicheKeywordProvider keywordProvider)
        {
            _keywordProvider = keywordProvider;
        }
        public bool IsRelevant(string niche, string productName)
        {
            var keywords = _keywordProvider.GetKeywords(niche);

            productName = productName.ToLower();

            return keywords.Any(k => productName.Contains(k));
        }
    }
}
