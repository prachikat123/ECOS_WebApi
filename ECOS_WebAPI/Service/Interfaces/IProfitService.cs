namespace ECOS_WebAPI.Service.Interfaces
{
    public interface IProfitService
    {
        decimal CalculateProfitPerUnit(decimal sellingPrice, decimal cost);
        decimal CalculateDailyProfit(decimal profit, int orders);
    }
}
