using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Services.Implementations;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICustomerService _customerService;

        public CustomerController(ApplicationDbContext context, ICustomerService customerService)
        {
            _context = context;
            _customerService = customerService;
        }

        //lasini-get customer addresses
        [HttpGet("{customerId}/address")]
        public async Task<IActionResult> GetCustomerAddress(Guid customerId)
        {
            var address = await _customerService.GetCustomerAddressAsync(customerId);
            if (address == null)
                return NotFound("Customer or address not found.");
            return Ok(address);
        }
    }
}
