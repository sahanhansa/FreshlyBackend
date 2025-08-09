using FreshlyBackendNew.Services.Implementations;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _servicesService;

        public ServiceController(IServiceService servicesService)
        {
            _servicesService = servicesService;
        }

        [HttpGet("GetServiceIdByName/{serviceName}")]
        public async Task<IActionResult> GetServiceIdByName(string serviceName)
        {
            var result = await _servicesService.GetServiceIdByNameAsync(serviceName);

            if (result == null)
                return NotFound($"Service with name '{serviceName}' not found.");

            return Ok(result);
        }
    }
}