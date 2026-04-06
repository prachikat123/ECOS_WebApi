using ECOS_WebAPI.Models;

namespace ECOS_WebAPI.Service.Interfaces
{
    public interface ISourcingAgent
    {
        Task<List<Supplier>> GetSuppliersAsync(string productName);

    }
}
