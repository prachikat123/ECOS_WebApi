using ECOS_WebAPI.Models.AI;

namespace ECOS_WebAPI.Models
{
    public class CreateAdRequest
    {
        public string AdAccountId { get; set; }   
        public string CampaignName { get; set; }
        public string AdSetName { get; set; }
        public string AdName { get; set; }
        public string PageId { get; set; }
        public string ImageUrl { get; set; }
        public string WebsiteUrl { get; set; }

        public decimal DailyBudget { get; set; }  
        public string CountryCode { get; set; }    
        public string Objective { get; set; }

        public int AgeMin { get; set; }
        public int AgeMax { get; set; }
        public string Gender { get; set; }
        public string Interests { get; set; }
        public string OptimizationGoal { get; set; }
        public string CampaignPlanName { get; set; }
        public string PrimaryText { get; set; }
        public string Headline { get; set; }
        public string CallToAction { get; set; }
        public string Keywords { get; set; }
        public string Platform { get; set; }

        public string LandingPageUrl { get; set; }
        public string Status { get; set; }
    }
}
