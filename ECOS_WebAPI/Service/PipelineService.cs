using System.Threading.Tasks;
using ECOS_WebAPI.Agents;
using ECOS_WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECOS_WebAPI.Service
{
    public class PipelineService
    {
        private readonly ResearchAgent _researchAgent;
        private readonly EvaluationAgent _evaluationAgent;


        public PipelineService(ResearchAgent researchAgent, EvaluationAgent evaluationAgent)
        {
            _researchAgent = researchAgent;
            _evaluationAgent = evaluationAgent;
        }
        public async Task<PipelineState> RunAsync(ResearchRequest request)
        {
            var state = new PipelineState
            {
                Niche = request.Niche,
                WebsiteUrl = request.WebsiteUrl,
                CurrentStep = "Research"
            };

            List<string> products = new List<string>();

            // case 1 : Website Url
            if (!string.IsNullOrEmpty(request.WebsiteUrl))
            {
                var websiteProducts = await _researchAgent.ExtractFromWebsite(request.WebsiteUrl);
                products.AddRange(websiteProducts);
            }

            //  Case 2: Niche
            if (!string.IsNullOrEmpty(request.Niche))
            {
                var nicheProducts = await _researchAgent.GetTrendingProducts(request.Niche);
                products.AddRange(nicheProducts);
            }

            products = products
           .Where(p => !p.ToLower().Contains("login"))
            .Where(p => !p.ToLower().Contains("password"))
            .Where(p => !p.ToLower().Contains("account"))
            .Distinct()
            .ToList();

            state.ResearchOutput = products;

            if (!products.Any() || products.Count <= 1)
            {
                if (!string.IsNullOrEmpty(request.Niche))
                {
                    var nicheProducts = await _researchAgent.GetTrendingProducts(request.Niche);
                    products.AddRange(nicheProducts);
                }
            }

            //Step 2 : Evaluation
            state.CurrentStep = "Evaluation";
            state.EvaluationOutput = await _evaluationAgent.EvaluateWithAI(products);

            state.IsApproved = state.EvaluationOutput.Any(p => p.TotalScore >= 50);

            return state;

        }
    }
}
