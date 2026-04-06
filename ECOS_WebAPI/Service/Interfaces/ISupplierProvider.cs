using ECOS_WebAPI.Models;

namespace ECOS_WebAPI.Service.Interfaces
{
    public interface ISupplierProvider
    {
        Task<List<Supplier>> GetSuppliersAsync(SupplierSearchRequest request);
    }
}
