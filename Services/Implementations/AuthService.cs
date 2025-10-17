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
using Microsoft.Extensions.Logging;

namespace FreshlyBackendNew.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly Dictionary<string, Func<ILoginStrategy>> _strategyFactories;

        public AuthService(
            ApplicationDbContext context, 
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;

            // Strategy factories with proper dependency injection
            _strategyFactories = new Dictionary<string, Func<ILoginStrategy>>
            {
                ["customer"] = () => new CustomerLoginStrategy(_context, this),
                ["driver"] = () => new DriverLoginStrategy(_context, this),
                ["laundry"] = () => new LaundryLoginStrategy(_context, this),
                ["admin"] = () => new AdminLoginStrategy(_context, this)
            };
        }

        public async Task<bool> IsUsernameTakenAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                return false;

            // ? OPTIMIZED: Single query with UNION
            var query = _context.Customers
                .Where(c => c.Username == username)
                .Select(c => c.Username)
                .Union(_context.Drivers.Where(d => d.Username == username).Select(d => d.Username))
                .Union(_context.Laundries.Where(l => l.Username == username).Select(l => l.Username))
                .Union(_context.Admins.Where(a => a.Username == username).Select(a => a.Username));

            return await query.AnyAsync();
        }

        public async Task<AuthResponse> LoginAsync(string userType, LoginData data)
        {
            if (!_strategyFactories.TryGetValue(userType.ToLower(), out var strategyFactory))
            {
                _logger.LogWarning("Unknown user type: {UserType}", userType);
                throw new ArgumentException($"Unknown user type: {userType}");
            }
            
            var strategy = strategyFactory();
            return await strategy.ExecuteAsync(data);
        }

        // Backward compatibility methods
        public async Task<AuthResponse> LoginCustomerAsync(LoginData loginData)
            => await LoginAsync("customer", loginData);

        public async Task<AuthResponse> LoginDriverAsync(LoginData loginData)
            => await LoginAsync("driver", loginData);

        public async Task<AuthResponse> LoginLaundryAsync(LoginData loginData)
            => await LoginAsync("laundry", loginData);

        public async Task<AuthResponse> LoginAdminAsync(LoginData loginData)
            => await LoginAsync("admin", loginData);

        public async Task EnsureDefaultAdminExistsAsync()
        {
            try
            {
                var existingAdmin = await _context.Admins
                    .FirstOrDefaultAsync(a => a.Username == "Sahan");

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

                    _logger.LogInformation("Default admin created successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ensuring default admin exists");
                throw;
            }
        }

        public async Task<bool> EditCustomerProfileAsync(CustomerProfileEditDto editDto)
        {
            try
            {
                var customer = await _context.Customers
                    .Include(c => c.Address)
                    .FirstOrDefaultAsync(c => c.CustomerId == editDto.CustomerId);

                if (customer == null)
                {
                    _logger.LogWarning("Customer not found: {CustomerId}", editDto.CustomerId);
                    return false;
                }

                if (customer.Username != editDto.Username && 
                    await IsUsernameTakenAsync(editDto.Username))
                {
                    _logger.LogWarning("Username already taken: {Username}", editDto.Username);
                    return false;
                }

                // Update Customer fields
                customer.FirstName = editDto.FirstName;
                customer.LastName = editDto.LastName;
                customer.Username = editDto.Username;
                customer.Email = editDto.Email;
                
                // Update or create Address
                if (customer.Address == null && !string.IsNullOrEmpty(editDto.HouseNo))
                {
                    customer.Address = new Address
                    {
                        AddressId = Guid.NewGuid(),
                        HouseNo = editDto.HouseNo,
                        Street = editDto.Street,
                        City = editDto.City,
                        PostalCode = editDto.PostalCode
                    };
                    _context.Addresses.Add(customer.Address);
                }
                else if (customer.Address != null)
                {
                    customer.Address.HouseNo = editDto.HouseNo;
                    customer.Address.Street = editDto.Street;
                    customer.Address.City = editDto.City;
                    customer.Address.PostalCode = editDto.PostalCode;
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Customer profile updated: {CustomerId}", editDto.CustomerId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing customer profile: {CustomerId}", editDto.CustomerId);
                return false;
            }
        }

        public async Task<bool> DeleteCustomerProfileAsync(Guid customerId)
        {
            try
            {
                var customer = await _context.Customers
                    .Include(c => c.Address)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                if (customer == null)
                {
                    _logger.LogWarning("Customer not found for deletion: {CustomerId}", customerId);
                    return false;
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Customer profile deleted: {CustomerId}", customerId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer profile: {CustomerId}", customerId);
                return false;
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
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"] ?? "30")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // ============================================================
    // LOGIN STRATEGIES
    // ============================================================

    public interface ILoginStrategy
    {
        Task<AuthResponse> ExecuteAsync(LoginData data);
    }

    public class CustomerLoginStrategy : ILoginStrategy
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;

        public CustomerLoginStrategy(ApplicationDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<AuthResponse> ExecuteAsync(LoginData loginData)
        {
            var customer = await _context.Customers
                .AsNoTracking()
                .Include(c => c.Address)
                .FirstOrDefaultAsync(u => u.Username == loginData.Username);

            if (customer == null || !BCrypt.Net.BCrypt.Verify(loginData.Password, customer.Password))
                return null;

            var claims = new Dictionary<string, string>
            {
                { "FirstName", customer.FirstName ?? string.Empty },
                { "LastName", customer.LastName ?? string.Empty },
                { "Email", customer.Email ?? string.Empty },
                { "HouseNo", customer.Address?.HouseNo ?? string.Empty },
                { "Street", customer.Address?.Street ?? string.Empty },
                { "City", customer.Address?.City ?? string.Empty },
                { "PostalCode", customer.Address?.PostalCode ?? string.Empty },
            };

            string token = _authService.GenerateJwtToken(customer.CustomerId.ToString(), customer.Username, claims);

            return new AuthResponse
            {
                Token = token,
                Username = customer.Username,
                UserId = customer.CustomerId.ToString()
            };
        }
    }

    public class DriverLoginStrategy : ILoginStrategy
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;

        public DriverLoginStrategy(ApplicationDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<AuthResponse> ExecuteAsync(LoginData loginData)
        {
            var driver = await _context.Drivers
                .AsNoTracking()
                .Include(d => d.Address)
                .FirstOrDefaultAsync(u => u.Username == loginData.Username);

            if (driver == null || !BCrypt.Net.BCrypt.Verify(loginData.Password, driver.Password))
                return null;

            var claims = new Dictionary<string, string>
            {
                { "FirstName", driver.FirstName ?? string.Empty },
                { "LastName", driver.LastName ?? string.Empty },
                { "Email", driver.Email ?? string.Empty },
                { "LicenseNo", driver.VehicleNo ?? string.Empty },
                { "HouseNo", driver.Address?.HouseNo ?? string.Empty },
                { "Street", driver.Address?.Street ?? string.Empty },
                { "City", driver.Address?.City ?? string.Empty },
                { "PostalCode", driver.Address?.PostalCode ?? string.Empty },
            };

            string token = _authService.GenerateJwtToken(driver.DriverId.ToString(), driver.Username, claims);

            return new AuthResponse
            {
                Token = token,
                Username = driver.Username,
                UserId = driver.DriverId.ToString()
            };
        }
    }

    public class LaundryLoginStrategy : ILoginStrategy
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;

        public LaundryLoginStrategy(ApplicationDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<AuthResponse> ExecuteAsync(LoginData loginData)
        {
            var laundry = await _context.Laundries
                .AsNoTracking()
                .Include(l => l.Address)
                .FirstOrDefaultAsync(u => u.Username == loginData.Username);

            if (laundry == null || !BCrypt.Net.BCrypt.Verify(loginData.Password, laundry.Password))
                return null;

            // Check account status
            if (laundry.AccountStatus != "active")
                return null;

            var claims = new Dictionary<string, string>
            {
                { "LaundryName", laundry.LaundryName ?? string.Empty },
                { "Email", laundry.Email ?? string.Empty },
                { "HouseNo", laundry.Address?.HouseNo ?? string.Empty },
                { "Street", laundry.Address?.Street ?? string.Empty },
                { "City", laundry.Address?.City ?? string.Empty },
                { "PostalCode", laundry.Address?.PostalCode ?? string.Empty },
            };

            string token = _authService.GenerateJwtToken(laundry.LaundryId.ToString(), laundry.Username, claims);

            return new AuthResponse
            {
                Token = token,
                Username = laundry.Username,
                UserId = laundry.LaundryId.ToString()
            };
        }
    }

    public class AdminLoginStrategy : ILoginStrategy
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;

        public AdminLoginStrategy(ApplicationDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<AuthResponse> ExecuteAsync(LoginData loginData)
        {
            var admin = await _context.Admins
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Username == loginData.Username);

            if (admin == null || !BCrypt.Net.BCrypt.Verify(loginData.Password, admin.Password))
                return null;

            var claims = new Dictionary<string, string>
            {
                { ClaimTypes.Role, admin.Role ?? string.Empty }
            };

            string token = _authService.GenerateJwtToken(admin.AdminId.ToString(), admin.Username, claims);

            return new AuthResponse
            {
                Token = token,
                Username = admin.Username,
                UserId = admin.AdminId.ToString()
            };
        }
    }
}