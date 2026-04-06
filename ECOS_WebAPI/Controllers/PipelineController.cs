using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECOS_WebAPI.Service;
using ECOS_WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using ECOS_WebAPI.Migrations;
using ECOS_WebAPI.Data;

namespace ECOS_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PipelineController : ControllerBase
    {
        private readonly PipelineService _pipelineService;
        private readonly AppDbContext _db;
        public PipelineController(PipelineService pipelineService, AppDbContext db)
        {
            _pipelineService = pipelineService;
            _db = db;
        }

        [HttpPost("run")]
        public async Task<IActionResult> RunPipeline([FromBody] RunPipelineRequest body)
        {
            //var userId = User.Identity.Name; // or from JWT

            //await _pipeline.RunAsync(input, userId);

            //return Ok("Pipeline executed successfully");

            var result = await _pipelineService.RunAsync(body.Request,body.Input);
            return Ok(result);

        }

        [HttpGet("test-db")]
        public async Task<IActionResult> TestDb()
        {
            var data = await _db.MetaConfigs.ToListAsync();
            return Ok(data);
        }

    }
}
