using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs.Driver_DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IDriverContactService _driverContactService;
        private readonly IDriverProfileService _driverProfileService;
        public DriverController(IDriverContactService driverContactService,IDriverProfileService driverProfileService)
        {
            _driverContactService = driverContactService;
            _driverProfileService = driverProfileService;
        }
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

        [HttpPost]
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

        [HttpGet("DriverProfile/{driverId}")]
        public async Task<IActionResult> DriverProfile(Guid driverId)
        {
            try
            {
                var driverProfile = await _driverProfileService.GetDriverProfileDetailsAsync(driverId);

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
