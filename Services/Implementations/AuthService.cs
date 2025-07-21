using FreshlyBackendNew.Services.Interfaces;
using FreshlyBackendNew.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FreshlyBackendNew.DTOs;
using BCrypt.Net;
using FreshlyBackendNew.Models;
using Microsoft.AspNetCore.Mvc;

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
                   await _context.Admins.AnyAsync(a => a.Username == username); // Include Admins
        }

        public async Task<AuthResponse> LoginCustomerAsync(LoginData loginData)
        {
            try
            {
                if (loginData?.Username == null || loginData.Password == null)
                    return null;

                var customer = await _context.Customers
                    .Include(c => c.Address)
                    .FirstOrDefaultAsync(u => u.Username == loginData.Username);

                if (customer == null || !BCrypt.Net.BCrypt.Verify(loginData.Password, customer.Password))
                    return null;

                string token = GenerateJwtToken(customer.CustomerId.ToString(), customer.Username ?? string.Empty, new Dictionary<string, string>
                {
                    { "FirstName", customer.FirstName ?? string.Empty },
                    { "LastName", customer.LastName ?? string.Empty },
                    { "Email", customer.Email ?? string.Empty },
                    { "HouseNo", customer.Address?.HouseNo ?? string.Empty },
                    { "Street", customer.Address?.Street ?? string.Empty },
                    { "City", customer.Address?.City ?? string.Empty },
                    { "PostalCode", customer.Address?.PostalCode ?? string.Empty },
                });

                return new AuthResponse
                {
                    Token = token,
                    Username = customer.Username,
                    UserId = customer.CustomerId.ToString()
                };
            }
            catch (Exception)
            {
                return null; // Avoid exposing exception details
            }
        }

        public async Task<AuthResponse> LoginLaundryAsync(LoginData loginData)
        {
            try
            {
                if (loginData?.Username == null || loginData.Password == null)
                    return null;

                var laundry = await _context.Laundries
                    .Include(l => l.Address)
                    .FirstOrDefaultAsync(u => u.Username == loginData.Username);

                if (laundry == null || !BCrypt.Net.BCrypt.Verify(loginData.Password, laundry.Password) || !(laundry.AccountStatus == "active"))
                    return null;

                string token = GenerateJwtToken(laundry.LaundryId.ToString(), laundry.Username ?? string.Empty, new Dictionary<string, string>
                {
                    { "LaundryName", laundry.LaundryName ?? string.Empty },
                    { "Email", laundry.Email ?? string.Empty },
                    { "HouseNo", laundry.Address?.HouseNo ?? string.Empty },
                    { "Street", laundry.Address?.Street ?? string.Empty },
                    { "City", laundry.Address?.City ?? string.Empty },
                    { "PostalCode", laundry.Address?.PostalCode ?? string.Empty },
                });

                return new AuthResponse
                {
                    Token = token,
                    Username = laundry.Username,
                    UserId = laundry.LaundryId.ToString()
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<AuthResponse> LoginAdminAsync(LoginData loginData)
        {
            try
            {
                if (loginData?.Username == null || loginData.Password == null)
                    return null;

                var admin = await _context.Admins
                    .FirstOrDefaultAsync(a => a.Username == loginData.Username);

                if (admin == null || !BCrypt.Net.BCrypt.Verify(loginData.Password, admin.Password))
                    return null;

                string token = GenerateJwtToken(admin.AdminId.ToString(), admin.Username ?? string.Empty, new Dictionary<string, string>());
                return new AuthResponse
                {
                    Token = token,
                    Username = admin.Username,
                    UserId = admin.AdminId.ToString()
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<AuthResponse> LoginDriverAsync(LoginData loginData)
        {
            try
            {
                if (loginData?.Username == null || loginData.Password == null)
                    return null;

                var driver = await _context.Drivers
                    .Include(d => d.Address)
                    .FirstOrDefaultAsync(u => u.Username == loginData.Username);

                if (driver == null || !BCrypt.Net.BCrypt.Verify(loginData.Password, driver.Password))
                    return null;

                string token = GenerateJwtToken(driver.DriverId.ToString(), driver.Username ?? string.Empty, new Dictionary<string, string>
                {
                    { "FirstName", driver.FirstName ?? string.Empty },
                    { "LastName", driver.LastName ?? string.Empty },
                    { "Email", driver.Email ?? string.Empty },
                    { "LicenseNo", driver.LicenseNo ?? string.Empty },
                    { "HouseNo", driver.Address?.HouseNo ?? string.Empty },
                    { "Street", driver.Address?.Street ?? string.Empty },
                    { "City", driver.Address?.City ?? string.Empty },
                    { "PostalCode", driver.Address?.PostalCode ?? string.Empty },
                });

                return new AuthResponse
                {
                    Token = token,
                    Username = driver.Username,
                    UserId = driver.DriverId.ToString()
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task EnsureDefaultAdminExistsAsync()
        {
            var existingAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.Username == "Sahan");
            if (existingAdmin == null)
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword("RCG");
                var admin = new Admin
                {
                    AdminId = Guid.NewGuid(),
                    Username = "Sahan",
                    Password = hashedPassword,
                    FirstName = "Sahan",
                    LastName = string.Empty,
                    Email = string.Empty,
                    Role = "SuperAdmin",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Admins.Add(admin);
                await _context.SaveChangesAsync();
            }
        }

        public string GenerateJwtToken(string userId, string username, Dictionary<string, string> additionalClaims)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username ?? string.Empty)
            };

            foreach (var claim in additionalClaims)
            {
                claims.Add(new Claim(claim.Key, claim.Value));
            }

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