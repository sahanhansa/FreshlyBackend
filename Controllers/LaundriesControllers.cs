
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaundriesController : ControllerBase
    {
        private readonly  ApplicationDbContext _context;

        public LaundriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Laundries>>> GetLaundries()
        {
            return await _context.Laundries.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<Laundries>>> PostLaundries(Laundries laundry)
        {
            _context.Laundries.Add(laundry);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLaundries), new { id = laundry.Id }, laundry);
        }
    }
}