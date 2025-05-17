using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IAllPickupService _pickupService;
        private readonly IAllDeliveryService _deliveryService;
        private readonly ApplicationDbContext _context;

        // Constructor with dependency injection for context and services
        public OrderController(ApplicationDbContext context, IAllPickupService pickupService, IAllDeliveryService deliveryService)
        {
            _context = context;
            _pickupService = pickupService;
            _deliveryService = deliveryService;
        }

        // GET: api/Order/GetAllPickups
      
        [HttpGet("GetAllPickups")]
        public async Task<IActionResult> GetAllPickups()
        {
            var pickups = await _pickupService.GetAllPickups();

            if (pickups == null)
            {
                return NotFound("No orders found.");
            }

            return Ok(pickups);
        }

        // GET: api/Order/GetAllPickups/{orderId}
        
        [HttpGet("GetAllPickups/{orderId}")]
        public async Task<IActionResult> GetPickupDetails(string orderId)
        {
            var pickupDetails = await _pickupService.GetPickupDetailsBYId(orderId);

            if (pickupDetails == null)
            {
                return NotFound("No orders found.");
            }

            return Ok(pickupDetails);
        }


        // GET: api/Order/GetAllDeliveries
  
        [HttpGet("GetAllDeliveries")]
        public async Task<IActionResult> GetAllDeliveries()
        {
            var delivery = await _deliveryService.GetAllDeliveries();

            
            if (delivery == null)
            {
                return NotFound("No orders found.");
            }

            return Ok(delivery);
        }



        // GET: api/Order/GetAllDeliveries/{orderId}
      
        [HttpGet("GetAllDeliveries/{orderId}")]
        public async Task<IActionResult> GetDeliveryDetails(string orderId)
        {
            var deliveryDetails = await _deliveryService.GetDeliveryDetailsBYId(orderId);

           
            if (deliveryDetails == null)
            {
                return NotFound("No orders found.");
            }

            return Ok(deliveryDetails);
        }
    }
}
