using ECOS_WebAPI.Models;

namespace ECOS_WebAPI.Service.Interfaces
{
    public interface IPricingService
    {
        PricingResult CalculatePrice(decimal landedCost, decimal? targetSellingPrice = null);
    }
}
