using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service;
using ECOS_WebAPI.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECOS_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetaController : ControllerBase
    {
        private readonly MetaCapiService _service;
        private readonly IMetaOnboardingService _metaService;

        public MetaController(MetaCapiService service, IMetaOnboardingService metaService)
        {
            _service = service;
            _metaService = metaService;
        }

        [HttpPost("track")]
        public async Task<IActionResult> TrackEvent([FromBody] LeadRequest request)
        {
            await _service.SendEvent(request);
            return Ok("Event Sent");
        }

        [HttpPost("connect")]
        public async Task<IActionResult> Connect([FromBody] MetaConfig request)
        {
            var userId = "user1";

            await _metaService.SaveMetaConfigAsync(userId, request.AccessToken);

            return Ok("Meta connected successfully");
        }
    }
}
