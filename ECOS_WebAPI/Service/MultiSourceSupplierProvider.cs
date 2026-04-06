using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service.Interfaces;

namespace ECOS_WebAPI.Service
{
    public class MultiSourceSupplierProvider 
    {
        private readonly IEnumerable<ISupplierProvider> _providers;

        public MultiSourceSupplierProvider(IEnumerable<ISupplierProvider> providers)
        {
            _providers = providers;
        }
        public async Task<List<Supplier>> GetSuppliersAsync(SupplierSearchRequest request)
        {
            var allSuppliers = new List<Supplier>();

            foreach (var provider in _providers)
            {
                var result = await provider.GetSuppliersAsync(request);
                if (result != null)
                    allSuppliers.AddRange(result);
            }

            return allSuppliers;
        }
    }
}
