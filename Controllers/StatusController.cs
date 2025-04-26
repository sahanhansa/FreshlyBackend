using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Status
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Status>>> GetStatuses()
        {
            return await _context.Statuses.ToListAsync();
        }

        // GET: api/Status/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Status>> GetStatus(Guid id)
        {
            var status = await _context.Statuses.FindAsync(id);
            if (status == null)
                return NotFound();

            return status;
        }

        // POST: api/Status
        [HttpPost]
        public async Task<ActionResult<Status>> CreateStatus(Status status)
        {
            _context.Statuses.Add(status);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStatus), new { id = status.StatusID }, status);
        }

        // PUT: api/Status/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(Guid id, Status status)
        {
            if (id != status.StatusID)
                return BadRequest();

            _context.Entry(status).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Statuses.Any(s => s.StatusID == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Status/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatus(Guid id)
        {
            var status = await _context.Statuses.FindAsync(id);
            if (status == null)
                return NotFound();

            _context.Statuses.Remove(status);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
