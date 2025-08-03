using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasicController : ControllerBase
    {
        private readonly IBasicService _basicService;
        private readonly ApplicationDbContext _context;

        public BasicController(IBasicService basicService, ApplicationDbContext context)
        {
            _basicService = basicService;
            _context = context;
        }

        [HttpGet("laundries-with-image")]
        public async Task<ActionResult<List<LaundryDetailsWithImageDTO>>> GetLaundriesWithImage()
        {
            try
            {
                var result = await _basicService.GetItemsByLaundryWithImageAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception (if you have logging)
                Console.WriteLine($"Error in controller: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving data.");
            }
        }
        // BasicController.cs

        [HttpGet("counts")]
        public async Task<ActionResult<TableCountsDto>> GetTableCounts()
        {
            try
            {
                var customerCount = await _context.Customers.CountAsync();
                var laundryCount = await _context.Laundries.CountAsync();
                var orderCount = await _context.Orders.CountAsync();

                var response = new TableCountsDto
                {
                    CustomerCount = customerCount,
                    LaundryCount = laundryCount,
                    OrderCount = orderCount
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception (use your preferred logging mechanism)
                Console.WriteLine($"Error fetching table counts: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while fetching table counts" });
            }
        }



    }
}
