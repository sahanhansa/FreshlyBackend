using FreshlyBackendNew.Services.Interfaces;
using FreshlyBackendNew.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Implementations
{
    public class AuthService(ApplicationDbContext context, IConfiguration configuration) : IAuthService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IConfiguration _configuration = configuration;

        public async Task<AuthResponse> LoginCustomerAsync(LoginData loginData)
        {
            try
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(u => u.Username == loginData.Username);
                if (customer == null || customer.Password != loginData.Password)
                {
                    return new AuthResponse
                    {
                        Token = string.Empty,
                        Username = string.Empty,
                        UserId = string.Empty
                    };
                }

                string token = GenerateJwtToken(customer.CustomerId.ToString(), customer.Username ?? string.Empty);

                return new AuthResponse
                {
                    Token = token,
                    Username = customer.Username ?? string.Empty,
                    UserId = customer.CustomerId.ToString()
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Token = ex.Message,
                    Username = "Error",
                    UserId = "Error"
                };
            }
        }

        public async Task<AuthResponse> LoginAdminAsync(LoginData loginData)
        {
            try
            {
                var admin = await _context.Owners.FirstOrDefaultAsync(u => u.Username == loginData.Username);
                if (admin == null || admin.Password != loginData.Password)
                {
                    return new AuthResponse
                    {
                        Token = string.Empty,
                        Username = string.Empty,
                        UserId = string.Empty
                    };
                }

                string token = GenerateJwtToken(admin.OwnerId.ToString(), admin.Username ?? string.Empty);

                return new AuthResponse
                {
                    Token = token,
                    Username = admin.Username ?? string.Empty,
                    UserId = admin.OwnerId.ToString()
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Token = ex.Message,
                    Username = "Error",
                    UserId = "Error"
                };
            }
        }

        public async Task<AuthResponse> LoginLaundryAsync(LoginData loginData)
        {
            try
            {
                var laundry = await _context.Laundries.FirstOrDefaultAsync(u => u.Username == loginData.Username);
                if (laundry == null || laundry.Password != loginData.Password)
                {
                    return new AuthResponse
                    {
                        Token = string.Empty,
                        Username = string.Empty,
                        UserId = string.Empty
                    };
                }

                string token = GenerateJwtToken(laundry.LaundryId.ToString(), laundry.Username ?? string.Empty);

                return new AuthResponse
                {
                    Token = token,
                    Username = laundry.Username ?? string.Empty,
                    UserId = laundry.LaundryId.ToString()
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Token = ex.Message,
                    Username = "Error",
                    UserId = "Error"
                };
            }
        }

        public async Task<AuthResponse> LoginDriverAsync(LoginData loginData)
        {
            try
            {
                var driver = await _context.Drivers.FirstOrDefaultAsync(u => u.Username == loginData.Username);
                if (driver == null || driver.Password != loginData.Password)
                {
                    return new AuthResponse
                    {
                        Token = string.Empty,
                        Username = string.Empty,
                        UserId = string.Empty
                    };
                }

                string token = GenerateJwtToken(driver.DriverId.ToString(), driver.Username ?? string.Empty);

                return new AuthResponse
                {
                    Token = token,
                    Username = driver.Username ?? string.Empty,
                    UserId = driver.DriverId.ToString()
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Token = ex.Message,
                    Username = "Error",
                    UserId = "Error"
                };
            }
        }

        public string GenerateJwtToken(string userId, string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username)
            };

            // Get JWT key from configuration with null check
            string jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Get issuer and audience with null checks
            string issuer = _configuration["Jwt:Issuer"] ?? "default_issuer";
            string audience = _configuration["Jwt:Audience"] ?? "default_audience";
            
            // Get expiration time with a default if not configured
            double expireMinutes = 30; // Default value
            if (_configuration["Jwt:ExpireMinutes"] != null)
            {
                expireMinutes = Convert.ToDouble(_configuration["Jwt:ExpireMinutes"]);
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
