namespace ECOS_WebAPI.Models.AI
{
    public class AIProductRequest
    {
        public EcosSetupInput Setup { get; set; }

        public string UserPrompt { get; set; }

        public decimal? OverrideBudget { get; set; }
        public bool? ForceHighMarginProducts { get; set; }

        public string Mode { get; set; }
        public bool GenerateImages { get; set; } = true;

        public bool AutoApprove { get; set; } = false;

       
    }
}
