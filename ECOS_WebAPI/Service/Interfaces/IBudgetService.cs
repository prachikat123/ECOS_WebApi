using ECOS_WebAPI.Models;

namespace ECOS_WebAPI.Service.Interfaces
{
    public interface IBudgetService
    {
        decimal CalculateDailyBudget(SourcingResult product);
    }
}
