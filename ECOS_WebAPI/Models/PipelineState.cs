namespace ECOS_WebAPI.Models
{
    public class PipelineState
    {
        public string Niche {  get; set; }

        //step outputs
        public string ResearchOutput {  get; set; }
        public string EvaluationOutput { get; set; }

        //control flags
        public bool IsApproved { get; set; }
        public string CurrentStep { get; set; }

    }
}
