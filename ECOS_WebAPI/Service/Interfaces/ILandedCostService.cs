using ECOS_WebAPI.Models;

namespace ECOS_WebAPI.Service.Interfaces
{
    public interface ILandedCostService
    {
        CostBreakdown Calculate(Supplier supplier);
    }
}
