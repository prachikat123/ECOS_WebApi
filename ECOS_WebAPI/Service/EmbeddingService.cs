namespace ECOS_WebAPI.Service
{
    public class EmbeddingService
    {
        private readonly OpenRouterService _aiService;

        public EmbeddingService(OpenRouterService aiService)
        {
            _aiService = aiService;
        }
        public async Task<double> GetSimilarity(string niche, string product)
        {
            var prompt = $@"
            Give similarity score between:
            Niche: {niche}
            Product: {product}

            Return ONLY number between 0 to 1
            ";

            var response = await _aiService.GetCompletion(prompt);

            if (double.TryParse(response, out double score))
                return score;

            return 0;
        }
    }
}
