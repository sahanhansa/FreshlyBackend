using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemCategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ItemCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔹 Create (POST)
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] ItemCategory category)
        {
            if (category == null || string.IsNullOrWhiteSpace(category.CategoryName))
            {
                return BadRequest("Invalid category data.");
            }

            _context.ItemCategories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, category);
        }

        // 🔹 Read All (GET)
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.ItemCategories
                .Include(c => c.Items) // Include related items
                .ToListAsync();
            return Ok(categories);
        }

        // 🔹 Read One (GET by ID)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.ItemCategories
                .Include(c => c.Items) // Include related items
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // 🔹 Update (PUT)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] ItemCategory updatedCategory)
        {
            var category = await _context.ItemCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Update fields
            category.CategoryName = updatedCategory.CategoryName;

            await _context.SaveChangesAsync();
            return Ok(category);
        }

        // 🔹 Delete (DELETE)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.ItemCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.ItemCategories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
