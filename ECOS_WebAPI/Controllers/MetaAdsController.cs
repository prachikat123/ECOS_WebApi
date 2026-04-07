using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service;
using ECOS_WebAPI.Service.Interfaces;
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
        public async Task<IActionResult> CreateFullAd([FromBody] CreateAdRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required");

            return Ok(await _service.CreateFullAdFlow(request));
        }
    }
}
