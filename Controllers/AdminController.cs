using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace FreshlyBackendNew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        // GET: api/Admin
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<IEnumerable<AdminDTO>>> GetAdmins()
        {
            try
            {
                var admins = await _adminService.GetAllAdminsAsync();
                return Ok(admins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admins");
                return StatusCode(500, new { Error = "An error occurred while retrieving admins" });
            }
        }

        // GET: api/Admin/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<AdminDTO>> GetAdmin(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid adminId))
                    return BadRequest(new { Error = "Invalid admin ID format" });

                var admin = await _adminService.GetAdminByIdAsync(adminId);
                if (admin == null)
                    return NotFound(new { Error = "Admin not found" });

                return Ok(admin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving admin with ID {id}");
                return StatusCode(500, new { Error = "An error occurred while retrieving the admin" });
            }
        }

        // POST: api/Admin
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<AdminDTO>> CreateAdmin(
            [FromForm] CreateAdminRequestDTO adminDTO, 
            IFormFile? profileImage)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var admin = await _adminService.CreateAdminAsync(adminDTO, profileImage);
                return CreatedAtAction(nameof(GetAdmin), new { id = admin.AdminId }, admin);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating admin");
                return StatusCode(500, new { Error = "An error occurred while creating the admin" });
            }
        }

        // PUT: api/Admin/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateAdmin(string id, AdminDTO adminDTO)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid adminId))
                    return BadRequest(new { Error = "Invalid admin ID format" });

                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var userRole = User.FindFirstValue(ClaimTypes.Role)!;

                var success = await _adminService.UpdateAdminAsync(adminId, adminDTO, userId, userRole);
                if (!success)
                    return NotFound(new { Error = "Admin not found" });

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating admin with ID {id}");
                return StatusCode(500, new { Error = "An error occurred while updating the admin" });
            }
        }

        // DELETE: api/Admin/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteAdmin(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid adminId))
                    return BadRequest(new { Error = "Invalid admin ID format" });

                var success = await _adminService.DeleteAdminAsync(adminId);
                if (!success)
                    return NotFound(new { Error = "Admin not found" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting admin with ID {id}");
                return StatusCode(500, new { Error = "An error occurred while deleting the admin" });
            }
        }

        // DELETE: api/Admin/delete-if-not-superadmin/{id}
        [HttpDelete("delete-if-not-superadmin/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteAdminIfNotSuperAdmin(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid adminId))
                    return BadRequest(new { Error = "Invalid admin ID format" });

                var success = await _adminService.DeleteAdminAsync(adminId);
                if (!success)
                    return NotFound(new { Error = "Admin not found" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                // If it's a SuperAdmin, the service will throw an exception
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting admin with ID {id}");
                return StatusCode(500, new { Error = "An error occurred while deleting the admin" });
            }
        }

        // POST: api/Admin/ResetPassword
        [HttpPost("ResetPassword")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ResetPassword(PasswordResetDTO resetDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!Guid.TryParse(resetDTO.AdminId, out Guid adminId))
                    return BadRequest(new { Error = "Invalid admin ID format" });

                var newPassword = await _adminService.ResetPasswordAsync(adminId);
                return Ok(new { Message = "Password has been reset", TemporaryPassword = newPassword });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting admin password");
                return StatusCode(500, new { Error = "An error occurred while resetting the password" });
            }
        }

        // POST: api/Admin/ForgotPassword
        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var (success, token, expiry) = await _adminService.ForgotPasswordAsync(forgotPasswordDTO.Email);
                
                // Always return success message for security
                return Ok(new
                {
                    Message = "If your email is registered, you will receive a password reset link",
                    Token = token, // Remove in production
                    ExpiresAt = expiry
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing forgot password request");
                return StatusCode(500, new { Error = "An error occurred while processing your request" });
            }
        }

        // POST: api/Admin/ResetPasswordWithToken
        [HttpPost("ResetPasswordWithToken")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordWithToken(ResetPasswordWithTokenDTO resetDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var success = await _adminService.ResetPasswordWithTokenAsync(
                    resetDTO.Email, resetDTO.Token, resetDTO.NewPassword);

                if (!success)
                    return BadRequest(new { Error = "Invalid email or token" });

                return Ok(new { Message = "Password has been successfully reset" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password with token");
                return StatusCode(500, new { Error = "An error occurred while resetting your password" });
            }
        }

        // GET: api/Admin/me
        [HttpGet("me")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<AdminDTO>> GetCurrentAdmin()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid adminId))
                return Unauthorized(new { Error = "Invalid or missing admin ID in token" });

            var admin = await _adminService.GetAdminByIdAsync(adminId);
            if (admin == null)
                return NotFound(new { Error = "Admin not found" });

            return Ok(admin);
        }
    }
}