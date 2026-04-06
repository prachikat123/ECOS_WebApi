using ECOS_WebAPI.Models;

namespace ECOS_WebAPI.Service.Interfaces
{
    public interface IMetaContextService
    {
        Task<MetaSettings> GetMetaContextAsync(string userId);
    }
}
