using  System.Threading.Tasks;
using ECOS_WebAPI.Service;
namespace ECOS_WebAPI.Agents
{
    public class EvaluationAgent
    {
        private readonly OpenRouterService _aiService;
        public EvaluationAgent(OpenRouterService aiService)
        {
            _aiService = aiService;
        }
        public async Task<string> EvaluateProduct(string researchOutput)
        {
            var prompt = $@"
            You are an e-commerce expert.

            From the following product ideas:
            {researchOutput}
            
            Do the following:
            1. Select the best product
            2. Explain why based on:
               - demand
               - competition
               - profit potential
            3. Give final decision: Approved or Rejected

            Keep answer simple.
            ";

            return await _aiService.SendPromptAsync(prompt);
        }

    }
}
