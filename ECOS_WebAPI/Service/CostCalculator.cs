using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service.Interfaces;
using Microsoft.Extensions.Options;
namespace ECOS_WebAPI.Service
{
    public class CostCalculator : ICostCalculator
    {
        private readonly SourcingSettings _settings;

        public CostCalculator(IOptions<SourcingSettings> settings)
        {
            _settings = settings.Value;
        }
        public CostBreakdown Calculate(Supplier supplier)
        {
            var productCost = supplier.UnitPrice;

            var shippingCost = _settings.DefaultShippingCostPerUnit;

            var customsCost = productCost * (_settings.CustomsPercentage / 100);

            var packagingCost = _settings.PackagingCostPerUnit;

            return new CostBreakdown
            {
                ProductCost = productCost,
                ShippingCost = shippingCost,
                CustomsCost = customsCost,
                PackagingCost = packagingCost
            };
        }
    }
}
