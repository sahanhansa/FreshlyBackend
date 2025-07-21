using FreshlyBackendNew.Services.Interfaces;
using FreshlyBackendNew.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requires JWT authentication
    public class CustomerProfileController : ControllerBase
    {
        private readonly IAuthService _authService;

        public CustomerProfileController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPut("profile")]
        public async Task<IActionResult> EditProfile([FromBody] CustomerProfileEditDto editDto)
        {
            // Ensure the authenticated user is editing their own profile
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null || editDto.CustomerId != Guid.Parse(userId))
                return Unauthorized("You can only edit your own profile.");

            var result = await _authService.EditCustomerProfileAsync(editDto);
            if (!result)
                return BadRequest("Failed to update profile.");

            return Ok("Profile updated successfully.");
        }

        [HttpDelete("profile")]
        public async Task<IActionResult> DeleteProfile()
        {
            // Get the authenticated user's ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("Invalid user.");

            var result = await _authService.DeleteCustomerProfileAsync(Guid.Parse(userId));
            if (!result)
                return BadRequest("Failed to delete profile.");

            return Ok("Profile deleted successfully.");
        }
    }
}