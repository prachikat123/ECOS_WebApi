using System.Threading.Tasks;
using ECOS_WebAPI.Agents;
using ECOS_WebAPI.Models;

namespace ECOS_WebAPI.Service
{
    public class PipelineService
    {
        private readonly ResearchAgent _researchAgent;

        public PipelineService(ResearchAgent researchAgent)
        {
            _researchAgent = researchAgent;
        }
        public async Task<PipelineState> RunAsync(string niche)
        {
            var state = new PipelineState
            {
                Niche = niche,
                CurrentStep = "Research"
            };
            state.ResearchOutput = await _researchAgent.GetTrendingProducts(niche);

            return state;

        }
    }
}
