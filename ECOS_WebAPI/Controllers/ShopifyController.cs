using ECOS_WebAPI.Models.AI;
using ECOS_WebAPI.Models.Shopify;
using ECOS_WebAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECOS_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopifyController : ControllerBase
    {
        private readonly ShopifyService _shopifyService;
        public ShopifyController(ShopifyService shopifyService)
        {
            _shopifyService = shopifyService;
        }

        [HttpPost("create-product")]
        public async Task<IActionResult> CreateProduct([FromBody] ShopifyProductRequest productRequest)
        {
            try
            {
                var result = await _shopifyService.CreateProductAsync(productRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
    }
}
