using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaundryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LaundryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lasini-get request for laundry list display
        [HttpGet]
        public async Task<IActionResult> GetLaundries()
        {
            var laundriesWithRatings = await _context.Laundries
                .Include(l => l.Address) // Ensure address is included
                .Include(l => l.Feedbacks) // Include feedbacks to calculate average rating
                .Select(l => new
                {
                    Laundry = l,
                    AverageRating = l.Feedbacks.Any() ? l.Feedbacks.Average(f => f.Rating) : 0
                })
                .ToListAsync();

            var dtoList = laundriesWithRatings.Select(l => new LaundryWithAddressDTO
            {
                LaundryId = l.Laundry.LaundryId.ToString(),
                LaundryName = l.Laundry.LaundryName,
                City = l.Laundry.Address.City,
                AverageRating = Math.Round(l.AverageRating ?? 0, 1) // Round to 1 decimal place
            }).ToList();

            return Ok(dtoList);
        }
    }
}
