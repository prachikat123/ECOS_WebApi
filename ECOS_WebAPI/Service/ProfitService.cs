using ECOS_WebAPI.Service.Interfaces;

namespace ECOS_WebAPI.Service
{
    public class ProfitService : IProfitService
    {
        public decimal CalculateProfitPerUnit(decimal sellingPrice, decimal cost)
        {
            return sellingPrice - cost;
        }

        public decimal CalculateDailyProfit(decimal profitPerUnit, int dailyOrders)
        {
            return profitPerUnit * dailyOrders;
        }
    }
}
