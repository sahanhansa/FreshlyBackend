using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Contact
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
        {
            return await _context.Contacts.ToListAsync();
        }

        // GET: api/Contact/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(Guid id, string contactNumber)
        {
            var contact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.ContactId == id && c.ContactNumber == contactNumber);

            if (contact == null)
            {
                return NotFound();
            }

            return contact;
        }

        // POST: api/Contact
        [HttpPost]
        public async Task<ActionResult<Contact>> PostContact(Contact contact)
        {
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContact), new { id = contact.ContactId, contactNumber = contact.ContactNumber }, contact);
        }

        // PUT: api/Contact/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact(Guid id, string contactNumber, Contact contact)
        {
            if (id != contact.ContactId || contactNumber != contact.ContactNumber)
            {
                return BadRequest();
            }

            _context.Entry(contact).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id, contactNumber))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Contact/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(Guid id, string contactNumber)
        {
            var contact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.ContactId == id && c.ContactNumber == contactNumber);

            if (contact == null)
            {
                return NotFound();
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContactExists(Guid id, string contactNumber)
        {
            return _context.Contacts.Any(c => c.ContactId == id && c.ContactNumber == contactNumber);
        }
    }
}
