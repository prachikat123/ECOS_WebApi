namespace ECOS_WebAPI.Models
{
    public class PipelineState
    {
        public string Niche {  get; set; }
        public string WebsiteUrl { get; set; }

        //step outputs
        public List<string> ResearchOutput { get; set; }
        public List<EvaluationModel> EvaluationOutput { get; set; }

        //control flags
        public bool IsApproved { get; set; }
        public string CurrentStep { get; set; }

    }
}
