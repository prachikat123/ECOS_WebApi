using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service.Interfaces;

namespace ECOS_WebAPI.Service
{
    public class BudgetService : IBudgetService
    {
        public decimal CalculateDailyBudget(SourcingResult product)
        {
            if (product == null || product.Cost == null)
                return 0;

            var profitPerUnit = product.SuggestedSellingPrice - product.Cost.TotalLandedCost;

            if (profitPerUnit <= 0)
                return 0;

            var estimatedDailySales = EstimateDailySales(product.Score);

            var budget = profitPerUnit * estimatedDailySales * 0.1m;

            budget = Math.Min(budget, 3000);

            return Math.Round(budget, 2);
        }

        private int EstimateDailySales(decimal score)
        {
            if (score >= 120) return 20;
            if (score >= 90) return 10;
            if (score >= 70) return 5;
            return 2;
        }
    }
}
