using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OwnerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔹 Create Owner (POST)
        [HttpPost]
        public async Task<IActionResult> CreateOwner([FromBody] Owner owner)
        {
            if (owner == null)
            {
                return BadRequest("Invalid owner data.");
            }

            _context.Owners.Add(owner);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOwner), new { id = owner.OwnerId }, owner);
        }

        // 🔹 Get All Owners (GET)
        [HttpGet]
        public async Task<IActionResult> GetOwners()
        {
            var owners = await _context.Owners.ToListAsync();
            return Ok(owners);
        }

        // 🔹 Get Single Owner (GET by ID)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOwner(Guid id)
        {
            var owner = await _context.Owners.FindAsync(id);
            if (owner == null)
            {
                return NotFound();
            }

            return Ok(owner);
        }

        // 🔹 Update Owner (PUT)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOwner(Guid id, [FromBody] Owner updatedOwner)
        {
            var owner = await _context.Owners.FindAsync(id);
            if (owner == null)
            {
                return NotFound();
            }

            // Update fields
            owner.FirstName = updatedOwner.FirstName;
            owner.LastName = updatedOwner.LastName;
            owner.OwnerName = updatedOwner.OwnerName;
            owner.Contact = updatedOwner.Contact;
            owner.Email = updatedOwner.Email;
            owner.HouseNo = updatedOwner.HouseNo;
            owner.Street = updatedOwner.Street;
            owner.City = updatedOwner.City;

            await _context.SaveChangesAsync();
            return Ok(owner);
        }

        // 🔹 Delete Owner (DELETE)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOwner(Guid id)
        {
            var owner = await _context.Owners.FindAsync(id);
            if (owner == null)
            {
                return NotFound();
            }

            _context.Owners.Remove(owner);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
