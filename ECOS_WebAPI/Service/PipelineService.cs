using System.Threading.Tasks;
using ECOS_WebAPI.Agents;
using ECOS_WebAPI.Models;

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
        public async Task<PipelineState> RunAsync(string niche)
        {
            var state = new PipelineState
            {
                Niche = niche,
                CurrentStep = "Research"
            };
            //Step 1 : Research
            state.ResearchOutput = await _researchAgent.GetTrendingProducts(niche);

            //Step 2 : Evaluation
            state.CurrentStep = "Evaluation";
            state.EvaluationOutput = await _evaluationAgent.EvaluateProduct(state.ResearchOutput);

            if (state.EvaluationOutput.ToLower().Contains("approved"))
            {
                state.IsApproved = true;
            }
            else
            {
                state.IsApproved = false;
            }

            return state;

        }
    }
}
