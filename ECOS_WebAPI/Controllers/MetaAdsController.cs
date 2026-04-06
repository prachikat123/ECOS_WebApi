using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECOS_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetaAdsController : ControllerBase
    {
        private readonly MetaAdsService _service;

        public MetaAdsController(MetaAdsService service)
        {
            _service = service;
        }

        [HttpPost("create-full-ad")]
        public async Task<IActionResult> CreateFullAd([FromForm] CreateAdRequest request)
        {
            var result = await _service.CreateFullAdFlow(request);
            return Ok(result);
        }
    }
}
