using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Service
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            return await _context.Services.ToListAsync();
        }

        // GET: api/Service/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetService(Guid id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();

            return service;
        }

        // POST: api/Service
        [HttpPost]
        public async Task<ActionResult<Service>> CreateService(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetService), new { id = service.ServiceId }, service);
        }

        // PUT: api/Service/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(Guid id, Service service)
        {
            if (id != service.ServiceId)
                return BadRequest();

            _context.Entry(service).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Services.Any(e => e.ServiceId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Service/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(Guid id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
