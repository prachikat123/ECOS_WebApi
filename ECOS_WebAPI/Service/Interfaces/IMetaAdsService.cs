using ECOS_WebAPI.Models;

namespace ECOS_WebAPI.Service.Interfaces
{
    public interface IMetaAdsService
    {
        //Task<string> CreateCampaignAsync(MetaCampaignRequest request);
        //Task<string> CreateAdSetAsync(MetaAdSetRequest request);
        //Task<string> CreateAdAsync(MetaAdRequest request);
        Task<string> CreateFullAdFlow(CreateAdRequest request);
    }
}
