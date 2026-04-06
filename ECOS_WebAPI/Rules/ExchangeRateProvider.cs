using ECOS_WebAPI.Models;
using ECOS_WebAPI.Rules.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;
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
            var url = $"https://api.exchangerate-api.com/v4/latest/{fromCurrency}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to fetch exchange rates");

            var content = await response.Content.ReadAsStringAsync();

            //Pare JSON
            using var json = JsonDocument.Parse(content);
            var root = json.RootElement;

            if (!root.TryGetProperty("rates", out var rates))
                throw new Exception("Invalid API response");

            if (!rates.TryGetProperty(toCurrency, out var rateElement))
                throw new Exception($"Exchange rate not found for {toCurrency}");

            return rateElement.GetDecimal();

            //var rates = new Dictionary<string, decimal>
            //    {
            //        { "USD_INR", 83.0m },
            //        { "INR_USD", 0.012m }
            //    };

            //var key = $"{fromCurrency}_{toCurrency}";

            //if (!rates.ContainsKey(key))
            //    throw new Exception("Exchange rate not found.");

            //return rates[key];

            //var url = $"https://api.exchangerate.host/latest?base={fromCurrency}&symbols={toCurrency}";
            //var response = await _httpClient.GetFromJsonAsync<ExchangeRateResponse>(url);

            //if (response?.Rates == null || !response.Rates.ContainsKey(toCurrency))
            //    throw new Exception("Exchange rate not found.");

            //return response.Rates[toCurrency];
        }
    }
}
