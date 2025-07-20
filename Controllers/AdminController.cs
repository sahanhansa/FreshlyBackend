using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FreshlyBackendNew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;
        private readonly IAuthService _authService;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger, IAuthService authService)
        {
            _context = context;
            _logger = logger;
            _authService = authService;
        }

        // GET: api/Admin
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<IEnumerable<AdminDTO>>> GetAdmins()
        {
            try
            {
                var admins = await _context.Admins.ToListAsync();
                
                return Ok(admins.Select(a => new AdminDTO
                {
                    AdminId = a.AdminId.ToString(),
                    Username = a.Username,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Email = a.Email,
                    Role = a.Role,
                    CreatedAt = a.CreatedAt,
                    LastLogin = a.LastLogin
                }));
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
                {
                    return BadRequest(new { Error = "Invalid admin ID format" });
                }

                var admin = await _context.Admins.FindAsync(adminId);

                if (admin == null)
                {
                    return NotFound(new { Error = "Admin not found" });
                }

                return Ok(new AdminDTO
                {
                    AdminId = admin.AdminId.ToString(),
                    Username = admin.Username,
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    Email = admin.Email,
                    Role = admin.Role,
                    CreatedAt = admin.CreatedAt,
                    LastLogin = admin.LastLogin
                });
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
        public async Task<ActionResult<AdminDTO>> CreateAdmin(AdminDTO adminDTO)
        {
            try
            {
                if (adminDTO == null || string.IsNullOrEmpty(adminDTO.Username) || string.IsNullOrEmpty(adminDTO.Password))
                {
                    return BadRequest(new { Error = "Username and password are required" });
                }

                // Check if the username is already taken
                if (await _context.Admins.AnyAsync(a => a.Username == adminDTO.Username))
                {
                    return Conflict(new { Error = "Username already exists" });
                }

                var admin = new Admin
                {
                    Username = adminDTO.Username,
                    Password = adminDTO.Password, // Plain text password
                    FirstName = adminDTO.FirstName,
                    LastName = adminDTO.LastName,
                    Email = adminDTO.Email,
                    Role = adminDTO.Role ?? "Admin",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Admins.Add(admin);
                await _context.SaveChangesAsync();

                // Return the created admin without the password
                return CreatedAtAction(
                    nameof(GetAdmin),
                    new { id = admin.AdminId.ToString() },
                    new AdminDTO
                    {
                        AdminId = admin.AdminId.ToString(),
                        Username = admin.Username,
                        FirstName = admin.FirstName,
                        LastName = admin.LastName,
                        Email = admin.Email,
                        Role = admin.Role,
                        CreatedAt = admin.CreatedAt
                    }
                );
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
                {
                    return BadRequest(new { Error = "Invalid admin ID format" });
                }

                var admin = await _context.Admins.FindAsync(adminId);

                if (admin == null)
                {
                    return NotFound(new { Error = "Admin not found" });
                }

                // Only allow SuperAdmin to update roles, or an admin to update their own profile
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userRole != "SuperAdmin" && userId != admin.AdminId.ToString())
                {
                    return Forbid();
                }

                // Update admin properties
                if (!string.IsNullOrEmpty(adminDTO.Username) && adminDTO.Username != admin.Username)
                {
                    // Check if the username is already taken
                    if (await _context.Admins.AnyAsync(a => a.Username == adminDTO.Username && a.AdminId != adminId))
                    {
                        return Conflict(new { Error = "Username already exists" });
                    }
                    admin.Username = adminDTO.Username;
                }

                if (!string.IsNullOrEmpty(adminDTO.Password))
                {
                    admin.Password = adminDTO.Password; // Plain text password
                    admin.PasswordResetToken = null;
                    admin.PasswordResetExpiry = null;
                }

                admin.FirstName = adminDTO.FirstName ?? admin.FirstName;
                admin.LastName = adminDTO.LastName ?? admin.LastName;
                admin.Email = adminDTO.Email ?? admin.Email;

                // Only SuperAdmin can update roles
                if (userRole == "SuperAdmin" && !string.IsNullOrEmpty(adminDTO.Role))
                {
                    admin.Role = adminDTO.Role;
                }

                await _context.SaveChangesAsync();

                return NoContent();
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
                {
                    return BadRequest(new { Error = "Invalid admin ID format" });
                }

                var admin = await _context.Admins.FindAsync(adminId);

                if (admin == null)
                {
                    return NotFound(new { Error = "Admin not found" });
                }

                // Prevent deleting the last SuperAdmin
                if (admin.Role == "SuperAdmin")
                {
                    var superAdminCount = await _context.Admins.CountAsync(a => a.Role == "SuperAdmin");
                    if (superAdminCount <= 1)
                    {
                        return BadRequest(new { Error = "Cannot delete the last SuperAdmin" });
                    }
                }

                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();

                return NoContent();
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
                if (resetDTO == null || string.IsNullOrEmpty(resetDTO.AdminId))
                {
                    return BadRequest(new { Error = "Admin ID is required" });
                }

                if (!Guid.TryParse(resetDTO.AdminId, out Guid adminId))
                {
                    return BadRequest(new { Error = "Invalid admin ID format" });
                }

                var admin = await _context.Admins.FindAsync(adminId);

                if (admin == null)
                {
                    return NotFound(new { Error = "Admin not found" });
                }

                // Generate a secure random password
                string newPassword = Guid.NewGuid().ToString().Substring(0, 8);
                admin.Password = newPassword; // Plain text password
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Password has been reset", TemporaryPassword = newPassword });
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
                if (forgotPasswordDTO == null || string.IsNullOrEmpty(forgotPasswordDTO.Email))
                {
                    return BadRequest(new { Error = "Email is required" });
                }

                var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == forgotPasswordDTO.Email);

                if (admin == null)
                {
                    // For security reasons, don't reveal that the email doesn't exist
                    return Ok(new { Message = "If your email is registered, you will receive a password reset link" });
                }

                // Generate a password reset token (a random string)
                string resetToken = Guid.NewGuid().ToString("N");
                
                // Set the token and expiration (24 hours from now)
                admin.PasswordResetToken = resetToken;
                admin.PasswordResetExpiry = DateTime.UtcNow.AddHours(24);
                
                await _context.SaveChangesAsync();

                // In a real application, you would send an email with the reset link
                // For development purposes, we'll return the token in the response
                // The frontend would use this token to allow the user to reset their password
                
                return Ok(new { 
                    Message = "Password reset link has been sent to your email",
                    Token = resetToken, // This would normally be sent via email, not in the response
                    ExpiresAt = admin.PasswordResetExpiry
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
                if (resetDTO == null || string.IsNullOrEmpty(resetDTO.Email) || 
                    string.IsNullOrEmpty(resetDTO.Token) || string.IsNullOrEmpty(resetDTO.NewPassword))
                {
                    return BadRequest(new { Error = "Email, token, and new password are required" });
                }

                var admin = await _context.Admins.FirstOrDefaultAsync(a => 
                    a.Email == resetDTO.Email && 
                    a.PasswordResetToken == resetDTO.Token);

                if (admin == null)
                {
                    return BadRequest(new { Error = "Invalid email or token" });
                }

                // Check if the token has expired
                if (admin.PasswordResetExpiry == null || admin.PasswordResetExpiry < DateTime.UtcNow)
                {
                    return BadRequest(new { Error = "Password reset token has expired" });
                }

                // Update the password and clear the reset token
                admin.Password = resetDTO.NewPassword; // Plain text password
                admin.PasswordResetToken = null;
                admin.PasswordResetExpiry = null;
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Password has been successfully reset" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password with token");
                return StatusCode(500, new { Error = "An error occurred while resetting your password" });
            }
        }
    }

    public class PasswordResetDTO
    {
        public string AdminId { get; set; }
    }

    public class ForgotPasswordDTO
    {
        public string Email { get; set; }
    }

    public class ResetPasswordWithTokenDTO
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}