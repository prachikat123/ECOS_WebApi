using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service.Interfaces;
using Microsoft.Extensions.Options;
namespace ECOS_WebAPI.Service
{
    public class SupplierScorer : ISupplierScorer
    {
        private readonly SourcingSettings _settings;

        public SupplierScorer(IOptions<SourcingSettings> settings)
        {
            _settings = settings.Value;
        }

        public decimal Score(Supplier supplier, CostBreakdown cost, decimal targetSellingPrice)
        {
            decimal score = 0;

            // Lower cost = higher score
            score += _settings.CostWeight * (1 / (cost.TotalLandedCost + 1));

            // Faster shipping = higher score
            score += _settings.ShippingWeight * (1 / (supplier.ShippingDays + 1));



            // Lower defect = higher score
            score += _settings.DefectWeight * Math.Max(0, (10 - supplier.DefectRate));

            // Branding bonus
            if (supplier.SupportsBranding)
                score += _settings.BrandingBonus;

            var profitMargin = (targetSellingPrice - cost.TotalLandedCost) / targetSellingPrice * 100;
            score += profitMargin;


            return Math.Round(score, 2);
        }
    }
}
