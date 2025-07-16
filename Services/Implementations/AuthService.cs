using FreshlyBackendNew.Services.Interfaces;
using FreshlyBackendNew.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FreshlyBackendNew.DTOs;
using BCrypt.Net;

namespace FreshlyBackendNew.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<bool> IsUsernameTakenAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                return false;

            return await _context.Customers.AnyAsync(c => c.Username == username) ||
                   await _context.Drivers.AnyAsync(d => d.Username == username) ||
                   await _context.Laundries.AnyAsync(l => l.Username == username) ||
                   await _context.Owners.AnyAsync(o => o.Username == username);
        }

        public async Task<AuthResponse> LoginCustomerAsync(LoginData loginData)
        {
            try
            {
                if (loginData?.Username == null || loginData.Password == null)
                    return null;

                var customer = await _context.Customers.FirstOrDefaultAsync(u => u.Username == loginData.Username);
                if (customer == null || !BCrypt.Net.BCrypt.Verify(loginData.Password, customer.Password))
                {
                    return null;
                }

                string token = GenerateJwtToken(customer.CustomerId.ToString(), customer.Username ?? string.Empty);

                return new AuthResponse
                {
                    Token = token,
                    Username = customer.Username,
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
                if (loginData?.Username == null || loginData.Password == null)
                    return null;

                var admin = await _context.Owners.FirstOrDefaultAsync(u => u.Username == loginData.Username);
                if (admin == null || !BCrypt.Net.BCrypt.Verify(loginData.Password, admin.Password))
                {
                    return null;
                }

                string token = GenerateJwtToken(admin.OwnerId.ToString(), admin.Username ?? string.Empty);

                return new AuthResponse
                {
                    Token = token,
                    Username = admin.Username,
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
                if (loginData?.Username == null || loginData.Password == null)
                    return null;

                var laundry = await _context.Laundries.FirstOrDefaultAsync(u => u.Username == loginData.Username);
                if (laundry == null || !BCrypt.Net.BCrypt.Verify(loginData.Password, laundry.Password))
                {
                    return null;
                }

                string token = GenerateJwtToken(laundry.LaundryId.ToString(), laundry.Username ?? string.Empty);

                return new AuthResponse
                {
                    Token = token,
                    Username = laundry.Username,
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
                if (loginData?.Username == null || loginData.Password == null)
                    return null;

                var driver = await _context.Drivers.FirstOrDefaultAsync(u => u.Username == loginData.Username);
                if (driver == null || !BCrypt.Net.BCrypt.Verify(loginData.Password, driver.Password))
                {
                    return null;
                }

                string token = GenerateJwtToken(driver.DriverId.ToString(), driver.Username ?? string.Empty);

                return new AuthResponse
                {
                    Token = token,
                    Username = driver.Username,
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
                new Claim(ClaimTypes.Name, username ?? string.Empty)
            };

            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT Key is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"] ?? "30")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}