using ECOS_WebAPI.Models;

namespace ECOS_WebAPI.Service.Interfaces
{
    public interface IAdDecisionService
    {
        bool ShouldRunAds(SourcingResult result);
    }
}
