using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrivilegeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PrivilegeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Privilege
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Privilege>>> GetPrivileges()
        {
            return await _context.Privileges.ToListAsync();
        }

        // GET: api/Privilege/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Privilege>> GetPrivilege(Guid id)
        {
            var privilege = await _context.Privileges.FindAsync(id);
            if (privilege == null)
                return NotFound();

            return privilege;
        }

        // POST: api/Privilege
        [HttpPost]
        public async Task<ActionResult<Privilege>> CreatePrivilege(Privilege privilege)
        {
            _context.Privileges.Add(privilege);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPrivilege), new { id = privilege.PrivilegeId }, privilege);
        }

        // PUT: api/Privilege/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrivilege(Guid id, Privilege privilege)
        {
            if (id != privilege.PrivilegeId)
                return BadRequest();

            _context.Entry(privilege).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Privileges.Any(p => p.PrivilegeId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Privilege/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrivilege(Guid id)
        {
            var privilege = await _context.Privileges.FindAsync(id);
            if (privilege == null)
                return NotFound();

            _context.Privileges.Remove(privilege);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
