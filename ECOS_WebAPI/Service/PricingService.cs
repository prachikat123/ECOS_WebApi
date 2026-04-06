using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service.Interfaces;
using Microsoft.Extensions.Options;
namespace ECOS_WebAPI.Service
{
    public class PricingService : IPricingService
    {
        
        public PricingResult CalculatePrice(decimal landedCost, decimal? targetSellingPrice = null )
        {
            decimal sellingPrice;

            if (targetSellingPrice.HasValue && targetSellingPrice > 0)
            {
                sellingPrice = targetSellingPrice.Value;
            }
            else
            {
                // fallback margin (30%)
                sellingPrice = landedCost * 1.3m;
            }

            var margin = ((sellingPrice - landedCost) / sellingPrice) * 100;


            return new PricingResult
            {
                SellingPrice = Math.Round(sellingPrice, 2),
                MarginPercentage = Math.Round(margin, 2)
            };

        }
    }
}
