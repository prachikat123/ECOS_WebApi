namespace ECOS_WebAPI.Models
{
    public class CurrencyConvertResponse
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal ConvertedAmount { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}
