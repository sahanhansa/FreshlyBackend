using System.Collections.Generic;
using System.Threading.Tasks;
using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserGroupController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserGroupController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UserGroup
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserGroup>>> GetUserGroups()
        {
            return await _context.UserGroups.ToListAsync();
        }

        // GET: api/UserGroup/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserGroup>> GetUserGroup(Guid id)
        {
            var userGroup = await _context.UserGroups.FindAsync(id);
            if (userGroup == null)
                return NotFound();

            return userGroup;
        }

        // POST: api/UserGroup
        [HttpPost]
        public async Task<ActionResult<UserGroup>> CreateUserGroup(UserGroup userGroup)
        {
            _context.UserGroups.Add(userGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserGroup), new { id = userGroup.UserGroupId }, userGroup);
        }

        // PUT: api/UserGroup/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserGroup(Guid id, UserGroup userGroup)
        {
            if (id != userGroup.UserGroupId)
                return BadRequest();

            _context.Entry(userGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.UserGroups.Any(ug => ug.UserGroupId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/UserGroup/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserGroup(Guid id)
        {
            var userGroup = await _context.UserGroups.FindAsync(id);
            if (userGroup == null)
                return NotFound();

            _context.UserGroups.Remove(userGroup);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
