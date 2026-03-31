namespace ECOS_WebAPI.Rules.Interfaces
{
    public interface ICurrencyMapper
    {
        Task<string> ConvertAsync(decimal amount, string fromCurrency, string toCurrency);
    }
}
