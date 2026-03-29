using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECOS_WebAPI.Service;
using ECOS_WebAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        [HttpPost]
        public async Task<IActionResult> RunPipeline([FromBody] ResearchRequest request)
        {
            var result = await _pipelineService.RunAsync(request);
            return Ok(result);

        }

    }
}
