using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Services;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IAllPickupService _pickupService;
        private readonly IAllDeliveryService _deliveryService;
        private readonly ICompleteTasksService _completetasksservice;
        private readonly ApplicationDbContext _context;

        public OrderController(
            IOrderService orderService,
            IAllPickupService pickupService,
            IAllDeliveryService deliveryService)
        {
            _orderService = orderService;
            _pickupService = pickupService;
            _deliveryService = deliveryService;
        }

        // lasini-get cutomer address when confirming order
        // GET: api/Order/Customer/{customerId}/address
        [HttpGet("Customer/{customerId}/address")]
        public async Task<IActionResult> GetCustomerAddress(Guid customerId)
        {
            try
            {
                var address = await _orderService.GetCustomerAddressAsync(customerId);

                if (address == null)
                    return NotFound($"No address found for customer with ID: {customerId}");

                return Ok(address);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
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

        [HttpGet("GetAllCompleteTasks/{driverId}")]
        public async Task<IActionResult> GetAllCompleteTask(Guid driverId)
        {
            try
            {
                var completetasks = await _completetasksservice.GetAllCompleteTasks(driverId);

                if (completetasks == null)
                {
                    return NotFound("No orders found.");
                }

                return Ok(completetasks);
            }
            catch (Exception ex)
            {
                // Log the exception here as needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpGet("GetAllCompleteTasksByOrderId/{orderId}")]
        public async Task<IActionResult> GetAllCompleteTaskDetails(string orderId)
        {
            var completetasksDetails = await _completetasksservice.GetAllCompleteTasksBYId(orderId);

            if (completetasksDetails == null)
            {
                return NotFound("No orders found.");
            }

            return Ok(completetasksDetails);
        }




        // --- Laundry Order Management (Rohansi) ---


        // GET: api/Order/{laundryId}/new-orders
        [HttpGet("{laundryId}/new-orders")]
        public async Task<IActionResult> GetNewOrders(Guid laundryId)
        {
            try
            {
                var orders = await _orderService.GetNewOrdersAsync(laundryId);

                if (orders == null || orders.Count == 0)
                    return NotFound($"No 'new' orders found for LaundryId: {laundryId}");

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // GET: api/Order/{laundryId}/processing-orders
        [HttpGet("{laundryId}/processing-orders")]
        public async Task<IActionResult> GetProcessingOrders(Guid laundryId)
        {
            try
            {
                var orders = await _orderService.GetProcessingOrdersAsync(laundryId);

                if (orders == null || orders.Count == 0)
                {
                    // Return 200 OK with empty list or a custom message
                    return Ok(new {
                        message = $"No 'Processing' orders found for LaundryId: {laundryId}",
                        orders = new List<OrderDTO>() // or whatever your order DTO type is
                    });
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        //Rohansi-Get completed orders
        [HttpGet("{laundryId}/completed-orders")]
        public async Task<IActionResult> GetCompletedOrders(Guid laundryId)
        {
            try
            {
                var orders = await _orderService.GetCompletedOrdersAsync(laundryId);

                if (orders == null || orders.Count == 0)
                    return NotFound($"No 'completed' orders found for LaundryId: {laundryId}");

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        
        // GET: api/Order/{laundryId}/all-orders
        [HttpGet("{laundryId}/all-orders")]
        public async Task<IActionResult> GetAllOrders(Guid laundryId)
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync(laundryId);

                if (orders == null || orders.Count == 0)
                    return NotFound($"No orders found!");

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        // GET: api/Order
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync(Guid.Empty);

                if (orders == null || orders.Count == 0)
                    return NotFound("No orders found!");

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // POST: api/Order - Create a new order
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDTO orderDto)
        {
            if (orderDto == null)
                return BadRequest("Order data is required.");

            try
            {
                var createdOrder = await _orderService.CreateOrderAsync(orderDto);
                if (createdOrder == null)
                    return StatusCode(500, "Failed to create order.");
                return CreatedAtAction(nameof(GetAllOrders), new { id = createdOrder.OrderId }, createdOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the order: {ex.Message}");
            }
        }

        // PUT: api/Order/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] OrderDTO orderDto)
        {
            if (orderDto == null || id != orderDto.OrderId)
            {
                return BadRequest("Invalid order data.");
            }

            try
            {
                var success = await _orderService.UpdateOrderAsync(id, orderDto);
                if (!success)
                {
                    return NotFound($"Order with ID {id} not found.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the order: {ex.Message}");
            }
        }

        // DELETE: api/Order/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            try
            {
                var success = await _orderService.DeleteOrderAsync(id);
                if (!success)
                {
                    return NotFound($"Order with ID {id} not found.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the order: {ex.Message}");
            }
        }

        [HttpPatch("MarksToTake")]
        public async Task<IActionResult> MarksToTake([FromBody] MarkOrderDto markOrder)
        {
            try
            {
                await _pickupService.MarksToTake(markOrder);

                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the order: {ex.Message}");
            }
        }

        [HttpPatch("MarksToDeliver")]
        public async Task<IActionResult> MarksToDeliver([FromBody] MarkOrderDto markOrder)
        {
            try
            {
                await _pickupService.MarksToDeliver(markOrder);

                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the order: {ex.Message}");
            }
        }

        [HttpPatch("MarksToLaundryTake")]
        public async Task<IActionResult> MarksToLaundryTake([FromBody] MarkOrderDto markOrder)
        {
            try
            {

                await _deliveryService.MarksToLaundryTake(markOrder);

                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the order: {ex.Message}");
            }
        }
        [HttpPatch("MarksToLaundryPick")]
        public async Task<IActionResult> MarksToLaundryPick([FromBody] MarkOrderDto orderDto)
        {
            try
            {
                await _deliveryService.MarksToLaundryPick(orderDto);

                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the order: {ex.Message}");
            }
        }
        [HttpPatch("MarksToCustomerDeliver")]
        public async Task<IActionResult> MarksToCustomerDeliver([FromBody] MarkOrderDto markOrder)
        {
            try
            {
                await _deliveryService.MarksToCustomerDeliver(markOrder);

                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the order: {ex.Message}");
            }
        }

        [HttpGet("{laundryId}/filtered-orders")]
        public async Task<IActionResult> GetFilteredOrders(Guid laundryId)
        {
            try
            {
                var orders = await _orderService.GetFilteredOrdersAsync(laundryId);

                if (orders == null || orders.Count == 0)
                    return NotFound("No orders found with the specified statuses.");

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
     

           //GET /api/order/count-by-status/{laundryId}/{statusId}
            [HttpGet("count-by-status/{laundryId}/{statusId}")]
            public async Task<ActionResult<int>> GetOrderCountByStatus(Guid laundryId, Guid statusId)
            {
                var count = await _orderService.GetOrderCountByStatusAsync(laundryId, statusId);
                return Ok(count);
            }
        }


    }

