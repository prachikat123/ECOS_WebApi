using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECOS_WebAPI.Service;
using ECOS_WebAPI.Models;
using System.Threading.Tasks;

namespace ECOS_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PipelineController : ControllerBase
    {
        private readonly PipelineService _pipelineService;
        public PipelineController(PipelineService pipelineService)
        {
            _pipelineService = pipelineService;
        }

        [HttpGet("research")]
        public async Task<IActionResult> RunPipeline([FromQuery] string niche)
        {
            if (string.IsNullOrWhiteSpace(niche))
            {
                return BadRequest(new
                {
                    message = "Niche is required"
                });
            }
            try
            {
                var result = await _pipelineService.RunAsync(niche);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Something went wrong",
                    error = ex.Message
                });
            }

        }

    }
}
