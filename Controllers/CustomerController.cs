using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController(ICustomerService customerService, ApplicationDbContext context) : ControllerBase
    {
        private readonly ICustomerService _customerService = customerService;
        private readonly ApplicationDbContext _context = context;

        // GET: api/Customer
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var customers = await _customerService.GetCustomerDetailsAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Customer/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(Guid id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    return NotFound("Customer not found.");
                }
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Customer/deleted
        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedCustomers()
        {
            try
            {
                var deletedCustomers = await _customerService.GetDeletedCustomersAsync();
                return Ok(deletedCustomers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Customer/restore/{id}
        [HttpPost("restore/{id}")]
        public async Task<IActionResult> RestoreCustomer(Guid id)
        {
            try
            {
                var restoredCustomer = await _customerService.RestoreCustomerAsync(id);
                if (restoredCustomer == null)
                {
                    return NotFound("Deleted customer not found.");
                }
                return Ok(restoredCustomer);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Customer
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerDto customerDto)
        {
            try
            {
                if (customerDto == null || !ModelState.IsValid)
                {
                    return BadRequest("Invalid customer data.");
                }

                var createdCustomer = await _customerService.CreateCustomerAsync(customerDto);
                return CreatedAtAction(nameof(GetCustomer), new { id = createdCustomer.CustomerId }, createdCustomer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Customer/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] CustomerDto customerDto)
        {
            try
            {
                if (customerDto == null || id != customerDto.CustomerId || !ModelState.IsValid)
                {
                    return BadRequest("Invalid customer data.");
                }

                var updatedCustomer = await _customerService.UpdateCustomerAsync(id, customerDto);
                if (updatedCustomer == null)
                {
                    return NotFound("Customer not found.");
                }
                return Ok(updatedCustomer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Customer/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id, [FromQuery] string reason = "Deleted by admin")
        {
            try
            {
                var deleted = await _customerService.DeleteCustomerAsync(id, reason);
                if (!deleted)
                {
                    return NotFound("Customer not found.");
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}