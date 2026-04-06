namespace ECOS_WebAPI.Service
{
    public class AIRelevanceService
    {
        private readonly OpenRouterService _aiService;

        public AIRelevanceService(OpenRouterService aiService)
        {
            _aiService = aiService;
        }

        public async Task<bool> IsRelevant(string niche, string productName)
        {
            var prompt = $@"
            Niche: {niche}
            Product: {productName}

            Answer ONLY YES or NO:
            Is this product relevant to this niche?
            ";

            var response = await _aiService.GetCompletion(prompt);

            return response.Trim().ToLower().Contains("yes");
        }
    }
}
