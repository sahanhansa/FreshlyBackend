using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Services.Interfaces;
using FreshlyBackendNew.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaundryController : ControllerBase
    {
        private readonly ILaundryService _laundryService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly ApplicationDbContext _context;

        public LaundryController(ILaundryService laundryService, IOrderDetailService orderDetailService, ApplicationDbContext context)
        
        {
            _laundryService = laundryService;
            _orderDetailService = orderDetailService;
            _context = context;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLaundries()
        {
            try
            {
                var dtoList = await _laundryService.GetLaundriesForCustomerAsync();
                return Ok(dtoList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllLaundries: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving laundries", details = ex.Message });
            }
        }

        [HttpGet("laundry-list-for-customer")]
        public async Task<IActionResult> GetLaundriesForCustomer()
        {
            try
            {
                var dtoList = await _laundryService.GetLaundriesForCustomerAsync();
                return Ok(dtoList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetLaundriesForCustomer: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving laundries", details = ex.Message });
            }
        }

        [HttpGet("laundry-list-for-admin")]
        public async Task<IActionResult> GetLaundriesForAdmin()
        {
            try
            {
                var dtoList = await _laundryService.GetLaundriesForAdminAsync();
                return Ok(dtoList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetLaundriesForAdmin: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving laundries for admin", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateLaundry([FromBody] LaundryAdminDTO laundryDto)
        {
            try
            {
                if (laundryDto == null)
                {
                    return BadRequest("Laundry data is required.");
                }
                var createdLaundry = await _laundryService.CreateLaundryAsync(laundryDto);
                if (string.IsNullOrEmpty(createdLaundry.LaundryId))
                {
                    return StatusCode(500, new { error = "Created laundry ID is missing" });
                }
                return CreatedAtAction(nameof(GetLaundryById), new { id = Guid.Parse(createdLaundry.LaundryId) }, createdLaundry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateLaundry: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while creating the laundry", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLaundryById(Guid id)
        {
            try
            {
                var laundry = await _laundryService.GetLaundryByIdAsync(id);
                if (laundry == null)
                {
                    return NotFound();
                }
                return Ok(laundry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetLaundryById: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving the laundry", details = ex.Message });
            }
        }

        [HttpPost("create-laundry-account")]
        public async Task<IActionResult> CreateLaundryAccount([FromBody] CreateLaundryAccountDTO dto)
        {
            if (dto == null)
                return BadRequest("Laundry account data is required.");
            var owner = new Owner
            {
                FirstName = dto.OwnerFirstName,
                LastName = dto.OwnerLastName,
                Email = dto.OwnerEmail,
                Password = dto.OwnerPassword,
                Username = dto.OwnerUsername
            };
            _context.Owners.Add(owner);
            await _context.SaveChangesAsync();
            var address = new Address
            {
                HouseNo = dto.HouseNo,
                Street = dto.Street,
                City = dto.City,
                PostalCode = dto.PostalCode
            };
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            var laundry = new Laundry
            {
                LaundryName = dto.LaundryName,
                Username = dto.LaundryUsername,
                Password = dto.LaundryPassword,
                Email = dto.LaundryEmail,
                AddressId = address.AddressId,
                OwnerId = owner.OwnerId,
                AccountStatus = "inactive"
            };
            _context.Laundries.Add(laundry);
            await _context.SaveChangesAsync();
            return Ok(new {
                LaundryId = laundry.LaundryId,
                OwnerId = owner.OwnerId,
                AddressId = address.AddressId,
                AccountStatus = laundry.AccountStatus
            });
        }

        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> ActivateLaundry(Guid id)
        {
            var laundry = await _context.Laundries.FindAsync(id);
            if (laundry == null)
                return NotFound(new { Error = "Laundry not found" });
            laundry.AccountStatus = "active";
            _context.Entry(laundry).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Laundry marked as active" });
        }

        [HttpPatch("{id}/delete")]
        public async Task<IActionResult> DeleteLaundryStatus(Guid id)
        {
            var laundry = await _context.Laundries.FindAsync(id);
            if (laundry == null)
                return NotFound(new { Error = "Laundry not found" });
            laundry.AccountStatus = "deleted";
            _context.Entry(laundry).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Laundry marked as deleted" });
        }

        [HttpGet("details/{laundryId}")]
        public async Task<IActionResult> GetLaundryDetails(Guid laundryId)
        {
            try
            {
                var laundryDetails = await _laundryService.GetLaundryDetailsAsync(laundryId);
                if (laundryDetails == null)
                {
                    return NotFound($"Laundry with ID {laundryId} not found.");
                }
                return Ok(laundryDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetLaundryDetails: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving the laundry details", details = ex.Message });
            }
        }

        [HttpGet("order-details/{laundryId}/{orderId}/{statusId}")]
        public async Task<IActionResult> GetOrderDetails(Guid laundryId, Guid orderId, Guid statusId)
        {
            try
            {
                // First verify that the order belongs to the specified laundry and has the specified status
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.OrderId == orderId && o.LaundryId == laundryId && o.StatusId == statusId);

                if (order == null)
                {
                    return NotFound($"Order with ID {orderId} not found for laundry {laundryId} with status {statusId}.");
                }

                // Get order details using the existing service
                var orderDetails = await _orderDetailService.GetOrderDetailsAsync(orderId);

                if (orderDetails == null)
                {
                    return NotFound($"Order details for order ID {orderId} not found.");
                }

                return Ok(orderDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetOrderDetails: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving the order details", details = ex.Message });
            }
        }

        [HttpPatch("order-details/{laundryId}/{orderId}/{statusId}")]
        public async Task<IActionResult> UpdateOrderStatus(Guid laundryId, Guid orderId, Guid statusId)
        {
            try
            {
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.OrderId == orderId && o.LaundryId == laundryId);

                if (order == null)
                {
                    return NotFound($"Order with ID {orderId} not found for laundry {laundryId}.");
                }

                order.StatusId = statusId;
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Order status updated successfully to {statusId}." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateOrderStatus: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while updating the order status", details = ex.Message });
            }
        }
    }
}