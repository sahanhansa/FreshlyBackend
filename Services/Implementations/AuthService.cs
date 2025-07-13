using FreshlyBackendNew.Services.Interfaces;
using FreshlyBackendNew.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FreshlyBackendNew.DTOs;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace FreshlyBackendNew.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResponse> LoginCustomerAsync(LoginData loginData)
        {
            try
            {
                if (loginData == null || string.IsNullOrEmpty(loginData.Username) || string.IsNullOrEmpty(loginData.Password))
                {
                    _logger.LogWarning("Login attempt with null or empty credentials");
                    return null;
                }

                var customer = await _context.Customers
                    .FirstOrDefaultAsync(u => u.Username == loginData.Username);

                if (customer == null)
                {
                    _logger.LogInformation($"Login attempt for non-existent customer username: {loginData.Username}");
                    return null;
                }

                // Note: This is a simple password comparison. 
                // In a production app, you should use a secure password hashing library
                if (customer.Password != loginData.Password)
                {
                    _logger.LogInformation($"Invalid password for customer username: {loginData.Username}");
                    return null;
                }

                string token = GenerateJwtToken(customer.CustomerId.ToString(), customer.Username);
                _logger.LogInformation($"Customer login successful: {customer.Username}");

                return new AuthResponse
                {
                    Token = token,
                    Username = customer.Username,
                    UserId = customer.CustomerId.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in LoginCustomerAsync: {ex.Message}");
                return new AuthResponse
                {
                    Token = "Error: " + ex.Message,
                    Username = "Error",
                    UserId = "Error"
                };
            }
        }

        public async Task<AuthResponse> LoginAdminAsync(LoginData loginData)
        {
            try
            {
                if (loginData == null || string.IsNullOrEmpty(loginData.Username) || string.IsNullOrEmpty(loginData.Password))
                {
                    _logger.LogWarning("Login attempt with null or empty credentials");
                    return null;
                }

                // First try to find the admin in the new Admin table
                var admin = await _context.Admins
                    .FirstOrDefaultAsync(u => u.Username == loginData.Username);

                if (admin != null)
                {
                    // Check if the password uses the BCrypt format
                    bool passwordValid;
                    
                    if (admin.Password.StartsWith("$2a$") || admin.Password.StartsWith("$2b$"))
                    {
                        // The password is hashed with BCrypt, verify it
                        try
                        {
                            passwordValid = BCrypt.Net.BCrypt.Verify(loginData.Password, admin.Password);
                        }
                        catch
                        {
                            // If BCrypt verification fails, fall back to plain text comparison
                            passwordValid = admin.Password == loginData.Password;
                        }
                    }
                    else
                    {
                        // Plain text comparison for backward compatibility
                        passwordValid = admin.Password == loginData.Password;
                    }
                    
                    if (!passwordValid)
                    {
                        _logger.LogInformation($"Invalid password for admin username: {loginData.Username}");
                        return null;
                    }

                    // Update the last login time
                    admin.LastLogin = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    string token = GenerateJwtToken(admin.AdminId.ToString(), admin.Username, admin.Role ?? "Admin");
                    _logger.LogInformation($"Admin login successful: {admin.Username}");

                    return new AuthResponse
                    {
                        Token = token,
                        Username = admin.Username,
                        UserId = admin.AdminId.ToString()
                    };
                }

                // Fallback to the Owner table for backward compatibility
                var owner = await _context.Owners
                    .FirstOrDefaultAsync(u => u.Username == loginData.Username);

                if (owner == null)
                {
                    _logger.LogInformation($"Login attempt for non-existent admin username: {loginData.Username}");
                    return null;
                }

                if (owner.Password != loginData.Password)
                {
                    _logger.LogInformation($"Invalid password for owner username: {loginData.Username}");
                    return null;
                }

                string ownerToken = GenerateJwtToken(owner.OwnerId.ToString(), owner.Username, "Owner");
                _logger.LogInformation($"Owner login as admin successful: {owner.Username}");

                return new AuthResponse
                {
                    Token = ownerToken,
                    Username = owner.Username,
                    UserId = owner.OwnerId.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in LoginAdminAsync: {ex.Message}");
                return new AuthResponse
                {
                    Token = "Error: " + ex.Message,
                    Username = "Error",
                    UserId = "Error"
                };
            }
        }

        public async Task<AuthResponse> LoginLaundryAsync(LoginData loginData)
        {
            try
            {
                if (loginData == null || string.IsNullOrEmpty(loginData.Username) || string.IsNullOrEmpty(loginData.Password))
                {
                    _logger.LogWarning("Login attempt with null or empty credentials");
                    return null;
                }

                var laundry = await _context.Laundries
                    .FirstOrDefaultAsync(u => u.Username == loginData.Username);

                if (laundry == null)
                {
                    _logger.LogInformation($"Login attempt for non-existent laundry username: {loginData.Username}");
                    return null;
                }

                if (laundry.Password != loginData.Password)
                {
                    _logger.LogInformation($"Invalid password for laundry username: {loginData.Username}");
                    return null;
                }

                string token = GenerateJwtToken(laundry.LaundryId.ToString(), laundry.Username, "Laundry");
                _logger.LogInformation($"Laundry login successful: {laundry.Username}");

                return new AuthResponse
                {
                    Token = token,
                    Username = laundry.Username,
                    UserId = laundry.LaundryId.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in LoginLaundryAsync: {ex.Message}");
                return new AuthResponse
                {
                    Token = "Error: " + ex.Message,
                    Username = "Error",
                    UserId = "Error"
                };
            }
        }

        public async Task<AuthResponse> LoginDriverAsync(LoginData loginData)
        {
            try
            {
                if (loginData == null || string.IsNullOrEmpty(loginData.Username) || string.IsNullOrEmpty(loginData.Password))
                {
                    _logger.LogWarning("Login attempt with null or empty credentials");
                    return null;
                }

                var driver = await _context.Drivers
                    .FirstOrDefaultAsync(u => u.Username == loginData.Username);

                if (driver == null)
                {
                    _logger.LogInformation($"Login attempt for non-existent driver username: {loginData.Username}");
                    return null;
                }

                if (driver.Password != loginData.Password)
                {
                    _logger.LogInformation($"Invalid password for driver username: {loginData.Username}");
                    return null;
                }

                string token = GenerateJwtToken(driver.DriverId.ToString(), driver.Username, "Driver");
                _logger.LogInformation($"Driver login successful: {driver.Username}");

                return new AuthResponse
                {
                    Token = token,
                    Username = driver.Username,
                    UserId = driver.DriverId.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in LoginDriverAsync: {ex.Message}");
                return new AuthResponse
                {
                    Token = "Error: " + ex.Message,
                    Username = "Error",
                    UserId = "Error"
                };
            }
        }

        // Helper method to hash passwords with BCrypt
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }

        // Helper method to verify if a password needs to be upgraded to BCrypt
        public bool PasswordNeedsUpgrade(string currentPassword)
        {
            return !(currentPassword.StartsWith("$2a$") || currentPassword.StartsWith("$2b$"));
        }

        // Updated to include role in token
        public string GenerateJwtToken(string userId, string username, string role = "User")
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username))
                {
                    _logger.LogError("Cannot generate token for empty userId or username");
                    throw new ArgumentException("UserId and username are required");
                }

                var jwtKey = _configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(jwtKey))
                {
                    _logger.LogError("JWT Key is not configured in appsettings.json");
                    throw new InvalidOperationException("JWT configuration is missing");
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating JWT token: {ex.Message}");
                throw;
            }
        }
    }
}
