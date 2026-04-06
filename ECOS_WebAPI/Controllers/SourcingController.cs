using ECOS_WebAPI.Agents;
using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECOS_WebAPI.Controllers
{
    [Route("api/sourcing")]
    [ApiController]
    public class SourcingController : ControllerBase
    {
        private readonly SourcingAgent _agent;

        public SourcingController(SourcingAgent agent)
        {
            _agent = agent;
        }

        [HttpPost]
        public async Task<IActionResult> Run([FromBody] SourcingApiRequest input)
        {
            var request = new SupplierSearchRequest
            {
                ProductName = input.ProductName,
                Country = input.Country,
                TargetMOQ = input.TargetMOQ
            };
            var result = await _agent.RunAsync(
                request,
                input.TargetSellingPrice,
                 input.ExpectedDailyOrders
            );

            return Ok(result);
        }
    }
}
