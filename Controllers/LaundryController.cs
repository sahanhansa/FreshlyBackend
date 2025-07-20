using FreshlyBackendNew.DTOs;
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

        [HttpGet]
        public async Task<IActionResult> GetAllLaundries()
        {
            try
            {
                // By default, return the customer-focused list when no specific endpoint is provided
                var dtoList = await _laundryService.GetLaundriesForCustomerAsync();
                return Ok(dtoList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllLaundries: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving laundries", details = ex.Message });
            }
        }

        [HttpGet("laundry-list-for-customer")]
        public async Task<IActionResult> GetLaundriesForCustomer()
        {
            try
            {
                var dtoList = await _laundryService.GetLaundriesForCustomerAsync();
                return Ok(dtoList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetLaundriesForCustomer: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving laundries", details = ex.Message });
            }
        }

        [HttpGet("laundry-list-for-admin")]
        public async Task<IActionResult> GetLaundriesForAdmin()
        {
            try
            {
                var dtoList = await _laundryService.GetLaundriesForAdminAsync();
                return Ok(dtoList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetLaundriesForAdmin: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving laundries for admin", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateLaundry([FromBody] LaundryAdminDTO laundryDto)
        {
            try
            {
                if (laundryDto == null)
                {
                    return BadRequest("Laundry data is required.");
                }
                var createdLaundry = await _laundryService.CreateLaundryAsync(laundryDto);
                
                // Add null check before parsing
                if (string.IsNullOrEmpty(createdLaundry.LaundryId))
                {
                    return StatusCode(500, new { error = "Created laundry ID is missing" });
                }
                
                return CreatedAtAction(nameof(GetLaundryById), new { id = Guid.Parse(createdLaundry.LaundryId) }, createdLaundry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateLaundry: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while creating the laundry", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLaundryById(Guid id)
        {
            try
            {
                var laundry = await _laundryService.GetLaundryByIdAsync(id);
                if (laundry == null)
                {
                    return NotFound();
                }
                return Ok(laundry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetLaundryById: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving the laundry", details = ex.Message });
            }
        }

        [HttpGet("details/{laundryId}")]
        public async Task<IActionResult> GetLaundryDetails(Guid laundryId)
        {
            try
            {
                var laundryDetails = await _laundryService.GetLaundryDetailsAsync(laundryId);
                if (laundryDetails == null)
                {
                    return NotFound($"Laundry with ID {laundryId} not found.");
                }
                return Ok(laundryDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetLaundryDetails: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving the laundry details", details = ex.Message });
            }
        }
    }
}