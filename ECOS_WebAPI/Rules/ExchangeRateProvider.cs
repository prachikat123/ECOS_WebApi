using System.Net.Http.Json;
using ECOS_WebAPI.Models;
using ECOS_WebAPI.Rules.Interfaces;
namespace ECOS_WebAPI.Rules
{
    public class ExchangeRateProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient;
        public ExchangeRateProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            var url = $"https://api.exchangerate.host/latest?base={fromCurrency}&symbols={toCurrency}";
            var response = await _httpClient.GetFromJsonAsync<ExchangeRateResponse>(url);

            if (response?.Rates == null || !response.Rates.ContainsKey(toCurrency))
                throw new Exception("Exchange rate not found.");

            return response.Rates[toCurrency];
        }
    }
}
