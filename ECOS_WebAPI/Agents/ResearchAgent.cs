using ECOS_WebAPI.Service;
using HtmlAgilityPack;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace ECOS_WebAPI.Agents
{
    public class ResearchAgent
    {
        private readonly OpenRouterService _aiService;

        public ResearchAgent(OpenRouterService aiService)
        {
            _aiService = aiService;
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
            return products
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
        public async Task<List<string>> GetTrendingProducts(string niche)
        {
            var prompt = $@"
                Give me 5 trending products in {niche} niche.

                IMPORTANT:
                - Return ONLY product names
                - No explanation
                - No numbering
                - No markdown
                - Output as comma-separated list

                Example:
                Product A, Product B, Product C";

            var response = await _aiService.SendPromptAsync(prompt);

            return response
                .Split(',')
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrEmpty(p))
                .ToList();
        }
    }
    
    }
