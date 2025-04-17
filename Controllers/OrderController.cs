using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (order == null)
                return BadRequest("Invalid data.");

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
        }

        // Read all
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders.ToListAsync();
            return Ok(orders);
        }

        // Read one
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // Update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] Order updatedOrder)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            order.Status = updatedOrder.Status;
            order.PickupDate = updatedOrder.PickupDate;
            order.PickupTime = updatedOrder.PickupTime;
            order.DeliveryDate = updatedOrder.DeliveryDate;
            order.DeliveryTime = updatedOrder.DeliveryTime;
            order.Date = updatedOrder.Date;
            order.Time = updatedOrder.Time;

            await _context.SaveChangesAsync();
            return Ok(order);
        }

        // Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
