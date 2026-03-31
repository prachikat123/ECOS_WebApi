using ECOS_WebAPI.Models;

namespace ECOS_WebAPI.Rules
{
    public class MarketRuleResolver
    {
        private readonly Dictionary<string, MarketRuleType> _countryRules;
        public MarketRuleResolver()
        {
            _countryRules = new Dictionary<string, MarketRuleType>(StringComparer.OrdinalIgnoreCase)
            {
                { "USA", MarketRuleType.Premium },
                { "Germany", MarketRuleType.Strict },
                { "France", MarketRuleType.Strict },
                { "India", MarketRuleType.Budget },
                { "Brazil", MarketRuleType.Budget }
            };
        }
        public MarketRuleType Resolve(string country)
        {
            if (string.IsNullOrWhiteSpace(country))
                return MarketRuleType.Default;

            if (_countryRules.TryGetValue(country, out var rule))
                return rule;
            return MarketRuleType.Default;
        }
        public List<MarketRule> Evaluate(Product product)
        {
            var rules = new List<MarketRule>();

            //price range
            rules.Add(new MarketRule
            {
                RuleName = "Price Range",
                IsPassed = product.Price >= 10 && product.Price <= 100,
                Reason = "Product should be between $10 and $100"

            });

            //Profit Margin 
            rules.Add(new MarketRule
            {
                RuleName = "Profit Margin",
                IsPassed = product.Price > product.CostPrice,
                Reason = "Selling price must be higher than cost"
            });
            return rules;
        }

    }
}
