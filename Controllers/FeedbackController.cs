using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Feedback
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacks()
        {
            return await _context.Feedbacks.ToListAsync();
        }

        // GET: api/Feedback/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Feedback>> GetFeedback(Guid id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
                return NotFound();

            return feedback;
        }

        // POST: api/Feedback
        [HttpPost]
        public async Task<ActionResult<Feedback>> CreateFeedback(Feedback feedback)
        {
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFeedback), new { id = feedback.FeedbackId }, feedback);
        }

        // PUT: api/Feedback/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFeedback(Guid id, Feedback feedback)
        {
            if (id != feedback.FeedbackId)
                return BadRequest();

            _context.Entry(feedback).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Feedbacks.Any(f => f.FeedbackId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Feedback/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(Guid id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
                return NotFound();

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
