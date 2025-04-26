using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderTypeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/OrderType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderType>>> GetOrderTypes()
        {
            return await _context.OrderTypes.ToListAsync();
        }

        // GET: api/OrderType/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderType>> GetOrderType(Guid id)
        {
            var orderType = await _context.OrderTypes.FindAsync(id);
            if (orderType == null)
                return NotFound();

            return orderType;
        }

        // POST: api/OrderType
        [HttpPost]
        public async Task<ActionResult<OrderType>> CreateOrderType(OrderType orderType)
        {
            _context.OrderTypes.Add(orderType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderType), new { id = orderType.TypeId }, orderType);
        }

        // PUT: api/OrderType/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderType(Guid id, OrderType orderType)
        {
            if (id != orderType.TypeId)
                return BadRequest();

            _context.Entry(orderType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.OrderTypes.Any(ot => ot.TypeId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/OrderType/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderType(Guid id)
        {
            var orderType = await _context.OrderTypes.FindAsync(id);
            if (orderType == null)
                return NotFound();

            _context.OrderTypes.Remove(orderType);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
