using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECOS_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly CurrencyService _currencyService;
        public CurrencyController(CurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        [HttpPost("convert")]
        public async Task<IActionResult> Convert(CurrencyConvertRequest request)
        {
            var result = await _currencyService.ConvertAsync(request);
            return Ok(result);
        }

    }
}
