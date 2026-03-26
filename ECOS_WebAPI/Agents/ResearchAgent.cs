using System.Threading.Tasks;
using ECOS_WebAPI.Service;
namespace ECOS_WebAPI.Agents
{
    public class ResearchAgent
    {
        private readonly OpenRouterService _aiService;

        public ResearchAgent(OpenRouterService aiService)
        {
            _aiService = aiService;
        }
        public async Task<string> GetTrendingProducts(string niche)
        {
            var prompt = $"Find 5 trending e-commerce products in {niche} niche";

            return await _aiService.SendPromptAsync(prompt);
        }
    }
    
    }
