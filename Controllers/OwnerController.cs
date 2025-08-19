using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.DTOs;
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

        [HttpGet("details/{laundryId}")]
        public async Task<IActionResult> GetOwnerDetails(Guid laundryId)
        {
            try
            {
                var laundry = await _context.Laundries
                    .Include(l => l.Owner)
                    .ThenInclude(o => o.Address)
                    .FirstOrDefaultAsync(l => l.LaundryId == laundryId);

                if (laundry?.Owner == null)
                {
                    return NotFound($"Owner details for laundry ID {laundryId} not found.");
                }

                var owner = laundry.Owner;

                // Create full address
                string fullAddress = string.Empty;
                if (owner.Address != null)
                {
                    var addressParts = new List<string>();
                    if (!string.IsNullOrEmpty(owner.Address.HouseNo)) addressParts.Add(owner.Address.HouseNo);
                    if (!string.IsNullOrEmpty(owner.Address.Street)) addressParts.Add(owner.Address.Street);
                    if (!string.IsNullOrEmpty(owner.Address.City)) addressParts.Add(owner.Address.City);
                    if (!string.IsNullOrEmpty(owner.Address.PostalCode)) addressParts.Add(owner.Address.PostalCode);

                    fullAddress = string.Join(", ", addressParts);
                }

                // Create full name
                var nameParts = new List<string>();
                if (!string.IsNullOrEmpty(owner.FirstName)) nameParts.Add(owner.FirstName);
                if (!string.IsNullOrEmpty(owner.LastName)) nameParts.Add(owner.LastName);
                string fullName = string.Join(" ", nameParts);

                var ownerDetails = new OwnerDetailsDTO
                {
                    OwnerId = owner.OwnerId.ToString(),
                    FirstName = owner.FirstName,
                    LastName = owner.LastName,
                    FullName = string.IsNullOrEmpty(fullName) ? null : fullName,
                    Email = owner.Email,
                    Address = fullAddress,
                    ContactNumber = "+94 71 234 5678" // This could be stored in the database
                };

                return Ok(ownerDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetOwnerDetails: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving the owner details", details = ex.Message });
            }
        }
    }
}
