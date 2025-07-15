using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Services;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreshlyBackendNew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IAllPickupService _pickupService;
        private readonly IAllDeliveryService _deliveryService;
        private readonly ApplicationDbContext _context;
        private readonly IOrderService _orderService;

        // Constructor with dependency injection
        public OrdersController(ApplicationDbContext context, IAllPickupService pickupService, IAllDeliveryService deliveryService, IOrderService orderService)
        {
            _context = context;
            _pickupService = pickupService;
            _deliveryService = deliveryService;
            _orderService = orderService;
        }

        //lasini-confirm new order
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmOrder([FromBody] ConfirmOrderDTO dto)
        {
            if (dto == null || dto.TemporaryOrderId == Guid.Empty)
                return BadRequest("Invalid order data.");

            try
            {
                var result = await _orderService.ConfirmOrderAsync(dto);
                if (!result)
                    return NotFound("Temporary order not found or has no items.");

                return Ok(new { message = "Order confirmed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while confirming the order: {ex.Message}");
            }
        }

        // --- Pickup Endpoints ---

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

        // --- Delivery Endpoints ---

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

        // --- Laundry Order Management (Rohansi) ---

        [HttpGet("{laundryId}/new-orders")]
        public async Task<IActionResult> GetNewOrders(Guid laundryId)
        {
            try
            {
                var orders = await _orderService.GetNewOrdersAsync(laundryId);

                if (orders == null || !orders.Any())
                    return NotFound($"No 'picked up' orders found for LaundryId: {laundryId}");

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{laundryId}/processing-orders")]
        public async Task<IActionResult> GetProcessingOrders(Guid laundryId)
        {
            try
            {
                var orders = await _orderService.GetProcessingOrdersAsync(laundryId);

                if (orders == null || !orders.Any())
                    return NotFound($"No 'Processing' orders found for LaundryId: {laundryId}");

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{laundryId}/all-orders")]
        public async Task<IActionResult> GetAllOrders(Guid laundryId)
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync(laundryId);

                if (orders == null || !orders.Any())
                    return NotFound($"No orders found!");

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        
    }
}
