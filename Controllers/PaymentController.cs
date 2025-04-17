using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPayment), new { id = payment.PaymentId }, payment);
        }

        // Read All
        [HttpGet]
        public async Task<IActionResult> GetPayments()
        {
            var payments = await _context.Payments.ToListAsync();
            return Ok(payments);
        }

        // Read One
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayment(Guid id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return NotFound();
            return Ok(payment);
        }

        // Update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(Guid id, [FromBody] Payment updatedPayment)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return NotFound();

            payment.Date = updatedPayment.Date;
            payment.Time = updatedPayment.Time;
            payment.Amount = updatedPayment.Amount;
            payment.Method = updatedPayment.Method;
            payment.Status = updatedPayment.Status;

            await _context.SaveChangesAsync();
            return Ok(payment);
        }

        // Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(Guid id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return NotFound();

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
