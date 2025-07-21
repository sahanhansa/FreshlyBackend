using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs.Admin;
using FreshlyBackendNew.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using FreshlyBackendNew.DTOs.Driver_DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using FreshlyBackendNew.Services.Interfaces;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IDriverContactService _driverContactService;
        private readonly IDriverProfileService _driverProfileService;
        private readonly IFileStorageService _fileStorageService;

        public DriverController(ApplicationDbContext context, IDriverContactService driverContactService, IDriverProfileService driverProfileService, IFileStorageService fileStorageService)
        {
            _context = context;
            _driverContactService = driverContactService;
            _driverProfileService = driverProfileService;
            _fileStorageService = fileStorageService;
        }

        // GET: api/Driver/GetContactUsDetails/{driverId}
        [HttpGet("GetContactUsDetails/{driverId}")]
        public async Task<IActionResult> GetContactUsDetails(Guid driverId)
        {
            try
            {
                var driverContact = await _driverContactService.GetDriverContactDetailsAsync(driverId);
                if (driverContact == null)
                    return NotFound($"No contact details found for driver with ID: {driverId}");

                return Ok(driverContact);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // POST: api/Driver/add-message
        [HttpPost("add-message")]
        public async Task<IActionResult> AddMessage(DriverContactDetailsDto driverContactDetailsDto)
        {
            try
            {
                await _driverContactService.AddMessage(driverContactDetailsDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // GET: api/Driver/DriverProfile/{driverId}
        [HttpGet("DriverProfile/{driverId}")]
        public async Task<IActionResult> DriverProfile(Guid driverId)
        {
            try
            {
                var driverProfile = await _driverProfileService.GetDriverProfileDetailsAsync(driverId);
                if (driverProfile == null)
                    return NotFound($"No profile found for driver with ID: {driverId}");

                return Ok(driverProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
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
                    LicenseNo = d.LicenseNo, // Only use LicenseNo property
                    AddressId = d.AddressId,
                    Address = d.Address == null ? null : new AddressDTO
                    {
                        AddressId = d.Address.AddressId,
                        HouseNo = d.Address.HouseNo,
                        Street = d.Address.Street,
                        City = d.Address.City,
                        PostalCode = d.Address.PostalCode
                    },
                    AccountStatus = d.AccountStatus,
                    ProfileImage = d.ProfileImage, // Include profile image URL
                    VehicleNo = d.VehicleNo // Include vehicle number
                })
                .ToListAsync();

            return Ok(drivers);
        }

        // POST: api/Driver - Adds a new driver with profile image upload
        [HttpPost]
        public async Task<IActionResult> CreateDriver([FromForm] CreateDriverRequestDTO dto, IFormFile profileImage)
        {
            if (dto == null)
                return BadRequest();

            string imageUrl = null;
            if (profileImage != null && profileImage.Length > 0)
            {
                imageUrl = await _fileStorageService.UploadFileAsync(profileImage, "driver-profile-images");
            }

            // Create and save Address
            var address = new Address
            {
                HouseNo = dto.HouseNo,
                Street = dto.Street,
                City = dto.City,
                PostalCode = dto.PostalCode
            };
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            var driver = new Driver
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Email = dto.Email,
                LicenseNo = dto.LicenseNo,
                AccountStatus = dto.AccountStatus ?? "active",
                ProfileImage = imageUrl,
                VehicleNo = dto.VehicleNo, // Set VehicleNo from DTO
                AddressId = address.AddressId // Assign AddressId
            };
            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();

            var response = new DriverProfileDTO
            {
                DriverId = driver.DriverId,
                FirstName = driver.FirstName,
                LastName = driver.LastName,
                Email = driver.Email,
                LicenseNo = driver.LicenseNo,
                AccountStatus = driver.AccountStatus,
                ProfileImage = driver.ProfileImage,
                VehicleNo = driver.VehicleNo, // Return VehicleNo in response
                AddressId = address.AddressId,
                Address = new AddressDTO
                {
                    AddressId = address.AddressId,
                    HouseNo = address.HouseNo,
                    Street = address.Street,
                    City = address.City,
                    PostalCode = address.PostalCode
                }
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

        [HttpGet("DriverEdit/{driverId}")]
        public async Task<IActionResult> DriverEdit(Guid driverId)
        {
            try
            {
                var driverProfile = await _driverProfileService.GetDriverEdit(driverId);

                if (driverProfile == null)
                    return NotFound($"No contact details found for driver with ID: {driverId}");

                return Ok(driverProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}