using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service.Interfaces;

namespace ECOS_WebAPI.Service
{
    public class NegotiationService: INegotiationService
    {
        public decimal GetNegotiatedPrice(Supplier supplier, int expectedDailyOrders)
        {
            decimal discount = 0;

            if (expectedDailyOrders > 300)
                discount = 0.10m; // 10%

            else if (expectedDailyOrders > 100)
                discount = 0.05m;

            var defectPenalty = supplier.DefectRate > 3 ? 0.03m : 0;

            return supplier.UnitPrice * (1 - discount - defectPenalty);
        }
    }
}
