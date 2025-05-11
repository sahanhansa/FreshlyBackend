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
    
    public class AuthService: IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        public async Task<AuthResponse> LoginCustomerAsync(LoginData loginData)
        {
            try
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(u => u.Username == loginData.Username);
                if (customer == null || customer.Password != loginData.Password)
                {
                    return null;
                }

                string token = GenerateJwtToken(customer.CustomerId.ToString(), customer.Username);

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
                var admin = await _context.Owners.FirstOrDefaultAsync(u => u.Username == loginData.Username);
                if (admin == null || admin.Password != loginData.Password)
                {
                    return null;
                }

                string token = GenerateJwtToken(admin.OwnerId.ToString(), admin.Username);

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
                var laundry = await _context.Laundries.FirstOrDefaultAsync(u => u.Username == loginData.Username);
                if (laundry == null || laundry.Password != loginData.Password)
                {
                    return null;
                }

                string token = GenerateJwtToken(laundry.LaundryId.ToString(), laundry.Username);

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
                var driver = await _context.Drivers.FirstOrDefaultAsync(u => u.Username == loginData.Username);
                if (driver == null || driver.Password != loginData.Password)
                {
                    return null;
                }

                string token = GenerateJwtToken(driver.DriverId.ToString(), driver.Username);

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
                new Claim(ClaimTypes.Name, username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
