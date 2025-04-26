using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
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

        // 🔹 Create (POST)
        [HttpPost]
        public async Task<IActionResult> CreateLaundry([FromBody] Laundry laundry)
        {
            if (laundry == null)
            {
                return BadRequest("Invalid laundry data.");
            }

            _context.Laundries.Add(laundry);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLaundry), new { id = laundry.LaundryId }, laundry);
        }

        // 🔹 Read All (GET)
        [HttpGet]
        public async Task<IActionResult> GetLaundries()
        {
            var laundries = await _context.Laundries.ToListAsync();
            return Ok(laundries);
        }

        // 🔹 Read One (GET by ID)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLaundry(Guid id)
        {
            var laundry = await _context.Laundries.FindAsync(id);
            if (laundry == null)
            {
                return NotFound();
            }
            return Ok(laundry);
        }

        // 🔹 Update (PUT)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLaundry(Guid id, [FromBody] Laundry updatedLaundry)
        {
            var laundry = await _context.Laundries.FindAsync(id);
            if (laundry == null)
            {
                return NotFound();
            }

            // Update fields
            laundry.LaundryName = updatedLaundry.LaundryName;
            laundry.Username = updatedLaundry.Username;
            laundry.Password = updatedLaundry.Password;
            laundry.Email = updatedLaundry.Email;
           
            await _context.SaveChangesAsync();
            return Ok(laundry);
        }

        // 🔹 Delete (DELETE)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLaundry(Guid id)
        {
            var laundry = await _context.Laundries.FindAsync(id);
            if (laundry == null)
            {
                return NotFound();
            }

            _context.Laundries.Remove(laundry);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
