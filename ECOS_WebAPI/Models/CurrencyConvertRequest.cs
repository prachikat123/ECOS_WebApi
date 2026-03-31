namespace ECOS_WebAPI.Models
{
    public class CurrencyConvertRequest
    {
        public string FromCurrency { get; set; }   
        public string ToCurrency { get; set; }     
        public decimal Amount { get; set; }
    }
}
