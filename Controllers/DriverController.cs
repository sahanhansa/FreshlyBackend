using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs.Admin;
using FreshlyBackendNew.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DriverController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Driver - Returns all drivers with required profile details
        [HttpGet]
        public async Task<IActionResult> GetDrivers()
        {
            var drivers = await _context.Drivers
                .Include(d => d.Address)
                .Select(d => new DriverProfileDTO
                {
                    DriverId = d.DriverId,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Email = d.Email,
                    LicensNo = d.LicensNo,
                    AddressId = d.AddressId,
                    Address = d.Address == null ? null : new AddressDTO
                    {
                        AddressId = d.Address.AddressId,
                        HouseNo = d.Address.HouseNo,
                        Street = d.Address.Street,
                        City = d.Address.City,
                        PostalCode = d.Address.PostalCode
                    },
                    AccountStatus = d.AccountStatus
                })
                .ToListAsync();
            return Ok(drivers);
        }

        // POST: api/Driver - Adds a new driver
        [HttpPost]
        public async Task<IActionResult> CreateDriver([FromBody] Driver driver)
        {
            if (driver == null)
                return BadRequest();

            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDrivers), new { id = driver.DriverId }, driver);
        }

        // POST: api/Driver/add-driver-with-address
        [HttpPost("add-driver-with-address")]
        public async Task<IActionResult> AddDriverWithAddress([FromBody] CreateDriverRequestDTO dto)
        {
            if (dto == null)
                return BadRequest();

            // Create Address
            var address = new Address
            {
                HouseNo = dto.HouseNo,
                Street = dto.Street,
                City = dto.City,
                PostalCode = dto.PostalCode
            };
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            // Create Driver
            var driver = new Driver
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Username = dto.Username,
                Password = dto.Password,
                Email = dto.Email,
                LicensNo = dto.LicensNo,
                AddressId = address.AddressId,
                AccountStatus = dto.AccountStatus ?? "active"
            };
            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();

            // Prepare response DTO
            var response = new DriverProfileDTO
            {
                DriverId = driver.DriverId,
                FirstName = driver.FirstName,
                LastName = driver.LastName,
                Email = driver.Email,
                LicensNo = driver.LicensNo,
                AddressId = address.AddressId,
                Address = new AddressDTO
                {
                    AddressId = address.AddressId,
                    HouseNo = address.HouseNo,
                    Street = address.Street,
                    City = address.City,
                    PostalCode = address.PostalCode
                },
                AccountStatus = driver.AccountStatus
            };
            return CreatedAtAction(nameof(GetDrivers), new { id = driver.DriverId }, response);
        }

        // PATCH: api/Driver/{id}/remove - Mark driver as deleted
        [HttpPatch("{id}/remove")]
        public async Task<IActionResult> RemoveDriver(Guid id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
                return NotFound(new { Error = "Driver not found" });

            driver.AccountStatus = "Deleted";
            _context.Entry(driver).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Driver marked as deleted" });
        }

        // PATCH: api/Driver/{id}/restore - Mark driver as active
        [HttpPatch("{id}/restore")]
        public async Task<IActionResult> RestoreDriver(Guid id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
                return NotFound(new { Error = "Driver not found" });

            driver.AccountStatus = "active";
            _context.Entry(driver).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Driver restored to active status" });
        }
    }
}
