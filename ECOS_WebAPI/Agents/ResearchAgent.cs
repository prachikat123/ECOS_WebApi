using ECOS_WebAPI.Models;
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

        public ResearchAgent(HttpClient httpClient, OpenRouterService aiService)
        {
            _httpClient = httpClient;
            _aiService = aiService;
        }

        private string _language = "en";
        public void SetLanguage(string language)
        {
            _language = language;
        }
        public async Task<List<string>> ExtractFromWebsite(string url)
        {
            var html = await GetWebsiteHtml(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var products = new List<string>();

            var nodes = doc.DocumentNode.SelectNodes("//h1 | //h2 | //h3 | //a");

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var text = node.InnerText.Trim().ToLower();

                    if (!string.IsNullOrEmpty(text) &&
                        text.Length > 5 &&
                        text.Length < 80 &&
                        !text.Contains("login") &&
                        !text.Contains("password") &&
                        !text.Contains("account") &&
                        !text.Contains("cart") &&
                        !text.Contains("share") &&
                        !text.Contains("tweet") &&
                        !text.Contains("home") &&
                        !text.Contains("shop") &&
                        !text.Contains("opening") &&
                        !text.Contains("enter") &&
                        !text.Contains("icon") &&
                        !text.Any(char.IsDigit))
                    {
                        products.Add(node.InnerText.Trim());
                    }
                }

            }
            var rawProducts = products
                     .Distinct()
                     .Take(10)
                     .ToList();

            var prompt = $@"
                From the following extracted website data, identify real e-commerce product ideas.

                IMPORTANT RULES:
                - Remove UI text (login, cart, navigation, home, shop, share, etc.)
                - Keep only real products
                - Return ONLY product names
                - No explanation
                - No numbering
                - Output comma-separated list
                - Language of output must be: {_language}

                DATA:
                {string.Join(", ", rawProducts)}
                ";

            var aiResult = await _aiService.GetCompletion(prompt);
            return aiResult
                .Split(',')
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrEmpty(p))
                .Distinct()
                .Take(10)
                .ToList();
        }

        private async Task<string> GetWebsiteHtml(string url)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

            return await client.GetStringAsync(url);
        }

        public async Task<List<ResearchOutput>> GenerateProductIdeas(string url)
        {
            var products = await ExtractFromWebsite(url);
            var prompt = $@"
                Convert these product names into structured e-commerce products.

                Return JSON array with:
                title, description, category, price

                Rules:
                - price must be numeric
                - description must be 1 line
                - category must be e-commerce category
                - return ONLY JSON array

                Data:
                {string.Join(",", products)}
                ";
            var aiResult = await _aiService.GetCompletion(prompt);
            var result = System.Text.Json.JsonSerializer.Deserialize<List<ResearchOutput>>(aiResult);
            return result ?? new List<ResearchOutput>();
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
