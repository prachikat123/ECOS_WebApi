using ECOS_WebAPI.Models;
using ECOS_WebAPI.Rules.Interfaces;

namespace ECOS_WebAPI.Service
{
    public class CurrencyService
    {
        private readonly IExchangeRateProvider _rateProvider;
        public CurrencyService(IExchangeRateProvider rateProvider)
        {
            _rateProvider = rateProvider;
        }
        public async Task<CurrencyConvertResponse> ConvertAsync(CurrencyConvertRequest request)
        {

            if (request.Amount <= 0)
                throw new ArgumentException("Amount must be greater than 0");

            if (request.FromCurrency == request.ToCurrency)
            {
                return new CurrencyConvertResponse
                {
                    FromCurrency = request.FromCurrency,
                    ToCurrency = request.ToCurrency,
                    OriginalAmount = request.Amount,
                    ConvertedAmount = request.Amount,
                    ExchangeRate = 1
                };
            }
            var rate = await _rateProvider.GetExchangeRateAsync(
                request.FromCurrency.ToUpper(),
                request.ToCurrency.ToUpper()
            );
            var convertedAmount = request.Amount * rate;
            return new CurrencyConvertResponse
            {
                FromCurrency = request.FromCurrency.ToUpper(),
                ToCurrency = request.ToCurrency.ToUpper(),
                OriginalAmount = request.Amount,
                ExchangeRate = rate,
                ConvertedAmount = convertedAmount
            };
        }
    }
}
