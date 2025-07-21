using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using FreshlyBackendNew.Data;

namespace FreshlyBackendNew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;
        private readonly ApplicationDbContext _context;

        public AuthController(IAuthService auth, ApplicationDbContext context)
        {
            _auth = auth;
            _context = context;
        }

        [HttpPost("customer/register")]
        public async Task<IActionResult> CustomerRegister([FromBody] CustomerRegisterDTO data)
        {
            try
            {
                if (await _auth.IsUsernameTakenAsync(data.Username))
                {
                    return BadRequest(new { Error = "Username already exists" });
                }

                if (await _context.Customers.AnyAsync(c => c.Email == data.Email))
                {
                    return BadRequest(new { Error = "Email already exists" });
                }

                var address = new Address
                {
                    HouseNo = data.HouseNo,
                    Street = data.Street,
                    City = data.City,
                    PostalCode = data.PostalCode
                };
                await _context.Addresses.AddAsync(address);

                var customer = new Customer
                {
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    Username = data.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(data.Password),
                    Email = data.Email,
                    Address = address
                };
                await _context.Customers.AddAsync(customer);

                var contact = new Contact
                {
                    ContactNumber = data.ContactNumber,
                    UserId = customer.CustomerId,
                    UserType = "Customer"
                };
                await _context.Contacts.AddAsync(contact);

                await _context.SaveChangesAsync();

                var authResponse = await _auth.LoginCustomerAsync(new LoginData
                {
                    Username = data.Username,
                    Password = data.Password
                });

                return Ok(new
                {
                    Token = authResponse.Token,
                    Username = authResponse.Username,
                    UserId = authResponse.UserId,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    HouseNo = address.HouseNo,
                    Street = address.Street,
                    City = address.City,
                    PostalCode = address.PostalCode,
                    ContactNumber = contact.ContactNumber
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("driver/register")]
        public async Task<IActionResult> DriverRegister([FromBody] DriverRegisterDTO data)
        {
            try
            {
                if (await _auth.IsUsernameTakenAsync(data.Username))
                {
                    return BadRequest(new { Error = "Username already exists" });
                }

                if (await _context.Drivers.AnyAsync(d => d.Email == data.Email))
                {
                    return BadRequest(new { Error = "Email already exists" });
                }

                var address = new Address
                {
                    HouseNo = data.HouseNo,
                    Street = data.Street,
                    City = data.City,
                    PostalCode = data.PostalCode
                };
                await _context.Addresses.AddAsync(address);

                var driver = new Driver
                {
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    Username = data.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(data.Password),
                    Email = data.Email,
                    LicenseNo = data.LicenseNo,
                    Address = address
                };
                await _context.Drivers.AddAsync(driver);

                var contact = new Contact
                {
                    ContactNumber = data.ContactNumber,
                    UserId = driver.DriverId,
                    UserType = "Driver"
                };
                await _context.Contacts.AddAsync(contact);

                await _context.SaveChangesAsync();

                var authResponse = await _auth.LoginDriverAsync(new LoginData
                {
                    Username = data.Username,
                    Password = data.Password
                });

                return Ok(new
                {
                    Token = authResponse.Token,
                    Username = authResponse.Username,
                    UserId = authResponse.UserId,
                    FirstName = driver.FirstName,
                    LastName = driver.LastName,
                    Email = driver.Email,
                    LicenseNo = driver.LicenseNo,
                    HouseNo = address.HouseNo,
                    Street = address.Street,
                    City = address.City,
                    PostalCode = address.PostalCode,
                    ContactNumber = contact.ContactNumber
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("laundry-owner/register")]
        public async Task<IActionResult> LaundryOwnerRegister([FromBody] LaundryOwnerRegisterDTO data)
        {
            try
            {
                if (await _auth.IsUsernameTakenAsync(data.Username))
                {
                    return BadRequest(new { Error = "Username already exists" });
                }

                if (await _context.Laundries.AnyAsync(l => l.Email == data.Email))
                {
                    return BadRequest(new { Error = "Laundry email already exists" });
                }

                if (await _context.Owners.AnyAsync(o => o.Email == data.OwnerEmail))
                {
                    return BadRequest(new { Error = "Owner email already exists" });
                }

                // Step 1: Save Owner Details
                var ownerAddress = new Address
                {
                    HouseNo = data.HouseNo,
                    Street = data.Street,
                    City = data.City,
                    PostalCode = data.PostalCode
                };
                await _context.Addresses.AddAsync(ownerAddress);

                var owner = new Owner
                {
                    FirstName = data.OwnerName,
                    LastName = data.LastName,
                    Email = data.OwnerEmail,
                    Address = ownerAddress
                };
                await _context.Owners.AddAsync(owner);

                var ownerContact = new Contact
                {
                    ContactNumber = data.OwnerContact,
                    UserId = owner.OwnerId,
                    UserType = "Owner"
                };
                await _context.Contacts.AddAsync(ownerContact);

                await _context.SaveChangesAsync(); // Save owner details to get OwnerId

                // Step 2: Save Laundry Details
                var laundryAddress = new Address
                {
                    HouseNo = data.StreetNumber,
                    Street = data.Street,
                    City = data.City,
                    PostalCode = data.PostalCode
                };
                await _context.Addresses.AddAsync(laundryAddress);

                var laundry = new Laundry
                {
                    LaundryName = data.LaundryName,
                    Username = data.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(data.Password),
                    Email = data.Email,
                    Address = laundryAddress,
                    OwnerId = owner.OwnerId, // Use the saved OwnerId
                    AccountStatus = "Not active"
                };
                await _context.Laundries.AddAsync(laundry);

                var laundryContact = new Contact
                {
                    ContactNumber = data.ContactNumber1,
                    UserId = laundry.LaundryId,
                    UserType = "Laundry"
                };
                await _context.Contacts.AddAsync(laundryContact);

                if (!string.IsNullOrEmpty(data.ContactNumber2))
                {
                    var secondaryContact = new Contact
                    {
                        ContactNumber = data.ContactNumber2,
                        UserId = laundry.LaundryId,
                        UserType = "Laundry"
                    };
                    await _context.Contacts.AddAsync(secondaryContact);
                }

                await _context.SaveChangesAsync(); // Save laundry details

                return Ok(new
                {
                    Message = "Laundry and owner registered successfully. Awaiting admin approval for login.",
                    LaundryId = laundry.LaundryId,
                    OwnerId = owner.OwnerId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("customer/login")]
        public async Task<IActionResult> CustomerLogin([FromBody] LoginData data)
        {
            try
            {
                var result = await _auth.LoginCustomerAsync(data);
                if (result == null)
                {
                    return BadRequest(new { Error = "Invalid username or password" });
                }

                var customer = await _context.Customers
                    .Include(c => c.Address)
                    .FirstOrDefaultAsync(c => c.CustomerId.ToString() == result.UserId);

                if (customer == null)
                {
                    return BadRequest(new { Error = "Customer not found" });
                }

                // Debugging: Check if Contacts are loaded
                
                

                return Ok(new
                {
                    Token = result.Token,
                    Username = result.Username,
                    UserId = result.UserId,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    HouseNo = customer.Address?.HouseNo,
                    Street = customer.Address?.Street,
                    City = customer.Address?.City,
                    PostalCode = customer.Address?.PostalCode,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("laundry/login")]
        public async Task<IActionResult> LaundryLogin([FromBody] LoginData data)
        {
            try
            {
                var result = await _auth.LoginLaundryAsync(data);
                if (result == null)
                {
                    return BadRequest(new { Error = "Invalid username or password or account not approved" });
                }

                var laundry = await _context.Laundries
                    .Include(l => l.Address)
                    .FirstOrDefaultAsync(l => l.LaundryId.ToString() == result.UserId);

                if (laundry == null || !(laundry.AccountStatus=="active"))
                {
                    return BadRequest(new { Error = "Laundry account not approved by admin" });
                }

                return Ok(new
                {
                    Token = result.Token,
                    Username = result.Username,
                    UserId = result.UserId,
                    LaundryName = laundry.LaundryName,
                    Email = laundry.Email,
                    HouseNo = laundry.Address?.HouseNo,
                    Street = laundry.Address?.Street,
                    City = laundry.Address?.City,
                    PostalCode = laundry.Address?.PostalCode,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("driver/login")]
        public async Task<IActionResult> DriverLogin([FromBody] LoginData data)
        {
            try
            {
                var result = await _auth.LoginDriverAsync(data);
                if (result == null)
                {
                    return BadRequest(new { Error = "Invalid username or password" });
                }

                var driver = await _context.Drivers
                    .Include(d => d.Address)
                    .FirstOrDefaultAsync(d => d.DriverId.ToString() == result.UserId);

                return Ok(new
                {
                    Token = result.Token,
                    Username = result.Username,
                    UserId = result.UserId,
                    FirstName = driver.FirstName,
                    LastName = driver.LastName,
                    Email = driver.Email,
                    LicenseNo = driver.LicenseNo,
                    HouseNo = driver.Address?.HouseNo,
                    Street = driver.Address?.Street,
                    City = driver.Address?.City,
                    PostalCode = driver.Address?.PostalCode,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginData data)
        {
            try
            {
                var result = await _auth.LoginAdminAsync(data);
                if (result == null)
                {
                    return BadRequest(new { Error = "Invalid username or password" });
                }

                var admin = await _context.Admins
                    .FirstOrDefaultAsync(a => a.AdminId.ToString() == result.UserId);

                if (admin == null)
                {
                    return BadRequest(new { Error = "Admin not found" });
                }

                return Ok(new
                {
                    Token = result.Token,
                    Username = result.Username,
                    UserId = result.UserId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}