using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs.Order_DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailController(
            ApplicationDbContext context,
            IOrderDetailService orderDetailService)
        {
            _context = context;
            _orderDetailService = orderDetailService;
        }

        //lasini- GET: api/OrderDetail/{orderId}
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetails(Guid orderId)
        {
            try
            {
                var orderDetails = await _orderDetailService.GetOrderDetailsAsync(orderId);

                if (orderDetails == null)
                    return NotFound($"Order with ID {orderId} not found.");

                return Ok(orderDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while retrieving order details: {ex.Message}");
            }
        }

        //lasini- GET: api/OrderDetail/customer/{customerId}/ongoing
        [HttpGet("customer/{customerId}/ongoing")]
        public async Task<IActionResult> GetOngoingOrders(Guid customerId)
        {
            try
            {
                var ongoingOrders = await _orderDetailService.GetOngoingOrdersForCustomerAsync(customerId);

                if (ongoingOrders == null || ongoingOrders.Count == 0)
                    return NotFound($"No ongoing orders found for customer with ID {customerId}.");

                return Ok(ongoingOrders);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while retrieving ongoing orders: {ex.Message}");
            }
        }

        // lasini-Add this endpoint for order cancellation
        [HttpDelete("cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            try
            {
                var result = await _orderDetailService.CancelOrderAsync(orderId);

                if (!result)
                    return NotFound($"Order with ID {orderId} not found or could not be canceled.");

                return Ok(new { message = "Order successfully canceled" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while canceling the order: {ex.Message}");
            }
        }

        //lasini
        // GET: api/OrderDetail/customer/{customerId}/outfordelivery
        [HttpGet("customer/{customerId}/outfordelivery")]
        public async Task<IActionResult> GetOutForDeliveryOrders(Guid customerId)
        {
            try
            {
                var outForDeliveryOrders = await _orderDetailService.GetOutForDeliveryOrdersForCustomerAsync(customerId);

                if (outForDeliveryOrders == null || outForDeliveryOrders.Count == 0)
                    return Ok(new List<OrderDetailsDTO>()); // Return empty list instead of 404 for easier client handling

                return Ok(outForDeliveryOrders);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while retrieving out for delivery orders: {ex.Message}");
            }
        }
    }
}