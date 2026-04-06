using ECOS_WebAPI.Models;

namespace ECOS_WebAPI.Service.Interfaces
{
    public interface ISupplierScorer
    {
        decimal Score(Supplier supplier, CostBreakdown cost, decimal targetSellingPrice);
    }
}
