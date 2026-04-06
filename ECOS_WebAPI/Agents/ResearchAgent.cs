using ECOS_WebAPI.Models;
using ECOS_WebAPI.Rules;
using ECOS_WebAPI.Service;
using HtmlAgilityPack;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace ECOS_WebAPI.Agents
{
    public class ResearchAgent
    {
        private readonly HttpClient _httpClient;
        private readonly OpenRouterService _aiService;
        private readonly ProductService _productService;
        private readonly ProductRelevanceRule _relevanceRule;
        private readonly AIRelevanceService _aiRelevanceService;
        private readonly CategoryDetector _categoryDetector;
        private readonly EmbeddingService _embeddingService;

        public ResearchAgent(HttpClient httpClient, OpenRouterService aiService, ProductService productService,
            ProductRelevanceRule relevanceRule, AIRelevanceService aiRelevanceService, CategoryDetector categoryDetector,
            EmbeddingService embeddingService)
        {
            _httpClient = httpClient;
            _aiService = aiService;
            _productService = productService;
            _relevanceRule = relevanceRule;
            _aiRelevanceService = aiRelevanceService;
            _categoryDetector = categoryDetector;
            _embeddingService = embeddingService;
        }

        private string _language = "en";
        public void SetLanguage(string language)
        {
            _language = language;
        }
        public async Task<List<string>> ExtractFromWebsite(string url)
        {
            var html = await GetWebsiteHtml(url);

            var rawProducts = ExtractRawProducts(html);

            var finalProducts = await FilterByNicheWithAI(rawProducts);

            return finalProducts;
        }
        private List<string> ExtractRawProducts(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var products = new List<string>();

            var nodes = doc.DocumentNode.SelectNodes("//h1 | //h2 | //h3 | //p | //span | //a");
            if (nodes == null)
                return products;

            foreach (var node in nodes)
            {
                var text = node.InnerText?.Trim();
                if (string.IsNullOrWhiteSpace(text))
                    continue;

                text = Regex.Replace(text, @"\s+", " ");
                var lower = text.ToLower();

                if (lower.Length < 5 || lower.Length > 80)
                    continue;

                if (Regex.IsMatch(lower, @"\d{4,}"))
                    continue;

                var blacklist = new[]
                {
                    "login","password","cart","checkout","account",
                    "home","shop","share","tweet","enter","icon",
                    "menu","footer","header","subscribe","copyright"
                };

                if (blacklist.Any(b => lower.Contains(b)))
                    continue;
                products.Add(text);
            }
            return products
                .Distinct()
                .Take(15)
                .ToList();

        }

        private async Task<List<string>> FilterByNicheWithAI(List<string> rawProducts)
        {
            var prompt = $@"
                You are an expert e-commerce product extraction system.

                TASK:
                Filter ONLY real sellable products relevant to the niche: {_language};

                RULES:
                - Remove all UI/navigation words (login, cart, home, shop, etc.)
                - Keep ONLY real, sellable products
                - Remove duplicates
                - If a word is not a product, ignore it
                - Output ONLY comma-separated product names
                - No explanation
                - No numbering
                - No markdown
                - No extra text

                INPUT LIST:
                {string.Join(", ", rawProducts)}
                ";

            var aiResult = await _aiService.GetCompletion(prompt);

            if (string.IsNullOrWhiteSpace(aiResult))
                return new List<string>();

            return aiResult
                 .Replace("\n", ",")
                 .Replace("-", ",")
                 .Split(',')
                 .Select(x => x.Trim())
                 .Where(x => !string.IsNullOrWhiteSpace(x))
                 .Distinct()
                 .Take(10)
                 .ToList();

        }
        private async Task<string> GetWebsiteHtml(string url)
        {
            try
            {

                _httpClient.Timeout = TimeSpan.FromSeconds(20);
                _httpClient.DefaultRequestHeaders.Clear();

                _httpClient.DefaultRequestHeaders.Add(
                    "User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64)"
                );

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return string.Empty;

                return await response.Content.ReadAsStringAsync();

            }
            catch
            {
                return string.Empty;
            }

        }

        public async Task<List<Product>> ConvertToCommercialProducts(List<string> products, EcosSetupInput input)
        {
            var prompt = $@"
                You are an expert e-commerce product cleaner.

                TASK:
                Return ONLY valid product names.

                INPUT:
                {string.Join(", ", products)}

                RULES:
                -No price
                - No description
                - No category
                - Only product names
                - Return comma-separated list
                ";

            var aiResult = await _aiService.GetCompletion(prompt);

            if (string.IsNullOrWhiteSpace(aiResult))
                return new List<Product>();

            var cleanProducts = aiResult
                .Replace("\n", ",")
                .Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            var result = new List<Product>();

            foreach (var name in cleanProducts)
            {
                if (!_relevanceRule.IsRelevant(input.Niche, name))
                    continue;

                bool isRelevant = await _aiRelevanceService.IsRelevant(input.Niche, name);

                if (!isRelevant)
                    continue;

                var score = await _embeddingService.GetSimilarity(input.Niche, name);

                if (score < 0.6)
                    continue;

                var category = _categoryDetector.Detect(name);

                if (category == "Other")
                    continue;

                var product = _productService.BuildProduct(name, input);
                product.Category = category;

                product.RelevanceScore = (int)(score * 100);

                result.Add(product);
            }

            return result;

        }


        public async Task<List<Product>> GenerateProductIdeas(string url, EcosSetupInput input)
        {
            var products = await ExtractFromWebsite(url);
            var commercialProducts = await ConvertToCommercialProducts(products, input);
            return commercialProducts;
        }


       
        public async Task<List<string>> GetTrendingProducts(string niche)
        {
            var prompt = $@"
                You are an expert e-commerce research agent.

                Give me 5 trending products in {niche} niche.

                Market Language: {_language}

                IMPORTANT:
                - Return ONLY product names
                - No explanation
                - No numbering
                - No markdown
                - Output as comma-separated list
                - No symbols or emojis
                
                Focus on:
                - High demand products
                - Profitable items
                - Current trending items in global market

                Example:
                Product A, Product B, Product C";

            var response = await _aiService.GetCompletion(prompt);

            return response
                .Split(',')
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrEmpty(p))
                .ToList();
        }
       
    }
    
}
