using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service.Interfaces;

namespace ECOS_WebAPI.Service
{
    public class AdDecisionService : IAdDecisionService
    {
        public bool ShouldRunAds(SourcingResult result)
        {
            return result.MarginPercentage > 25
           && !result.RiskFlags.Any();
        }
    }
}
