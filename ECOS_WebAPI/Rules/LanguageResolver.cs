namespace ECOS_WebAPI.Rules
{
    public class LanguageResolver
    {
        private readonly Dictionary<string, string> _countryLanguageMap;
        public LanguageResolver()
        {
            _countryLanguageMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "USA", "en" },
                { "UK", "en" },
                { "India", "en" },
                { "Germany", "de" },
                { "France", "fr" },
                { "Spain", "es" },
                { "Brazil", "pt" },
                { "Italy", "it" }
            };
        }
        public string Resolve(string country)
        {
            if (string.IsNullOrWhiteSpace(country))
                return "en";
            if (_countryLanguageMap.TryGetValue(country, out var language))
                return language;
            return "en";
        }
    }
}
