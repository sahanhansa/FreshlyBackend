using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaundryController : ControllerBase
    {
        private readonly ILaundryService _laundryService;

        public LaundryController(ILaundryService laundryService)
        {
            _laundryService = laundryService;
        }

        //Lasini- GET request to get laundry list
        [HttpGet("laundry-list-for-customer")]
        public async Task<IActionResult> GetLaundriesForCustomer()
        {
            try
            {
                // Fetch the list of laundries using the service
                var dtoList = await _laundryService.GetLaundriesForCustomerAsync();
                return Ok(dtoList);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                Console.WriteLine($"Error in GetLaundriesForCustomer: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving laundries", details = ex.Message });
            }
        }

    }
}
