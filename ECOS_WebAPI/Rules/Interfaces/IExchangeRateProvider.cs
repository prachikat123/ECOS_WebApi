namespace ECOS_WebAPI.Rules.Interfaces
{
    public interface IExchangeRateProvider
    {
        Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency);
    }
}
