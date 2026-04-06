using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service.Interfaces;
namespace ECOS_WebAPI.Service
{
    public class AlibabaSupplierProvider : ISupplierProvider
    {
        public async Task<List<Supplier>> GetSuppliersAsync(SupplierSearchRequest request)
        {
            //Mock  data
            var suppliers = new List<Supplier>
            {
                new Supplier
                {
                    Name = "Alibaba Supplier A",
                    Platform = "Alibaba",
                    UnitPrice = 500,
                    MOQ = 100,
                    ShippingDays = 7,
                    DefectRate = 2,
                    SupportsBranding = true
                },
                new Supplier
                {
                    Name = "Alibaba Supplier B",
                    Platform = "Alibaba",
                    UnitPrice = 480,
                    MOQ = 200,
                    ShippingDays = 9,
                    DefectRate = 3,
                    SupportsBranding = true
                }
            };

            return suppliers
                .ToList();
           
        }
    }
}
