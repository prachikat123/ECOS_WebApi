using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service.Interfaces;

namespace ECOS_WebAPI.Service
{
    public class LandedCostService : ILandedCostService
    {
        public CostBreakdown Calculate(Supplier supplier)
        {
            var productCost = supplier.UnitPrice;
            var shipping = supplier.UnitPrice * 0.10m;
            var customs = supplier.UnitPrice * 0.05m;
            var packaging = supplier.UnitPrice * 0.02m;

            return new CostBreakdown
            {
                ProductCost = productCost,
                ShippingCost = shipping,
                CustomsCost = customs,
                PackagingCost = packaging,
                TotalLandedCost = productCost + shipping + customs + packaging
            };
        }
    }
}
