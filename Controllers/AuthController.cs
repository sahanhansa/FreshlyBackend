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
                    UserId = authResponse.UserId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = ex.Message
                });
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
                    LicensNo = data.LicenseNo,
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
                    UserId = authResponse.UserId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = ex.Message
                });
            }
        }

        [HttpPost("laundry/register")]
        public async Task<IActionResult> LaundryRegister([FromBody] LaundryRegisterDTO data)
        {
            try
            {
                if (await _auth.IsUsernameTakenAsync(data.Username))
                {
                    return BadRequest(new { Error = "Username already exists" });
                }

                if (await _context.Laundries.AnyAsync(l => l.Email == data.Email))
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

                var owner = await _context.Owners.FirstOrDefaultAsync(o => o.OwnerId == data.OwnerId);
                if (owner == null)
                {
                    return BadRequest(new { Error = "Invalid OwnerId" });
                }

                var laundry = new Laundry
                {
                    LaundryName = data.LaundryName,
                    Username = data.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(data.Password),
                    Email = data.Email,
                    Address = address,
                    Owner = owner
                };
                await _context.Laundries.AddAsync(laundry);

                var contact = new Contact
                {
                    ContactNumber = data.ContactNumber,
                    UserId = laundry.LaundryId,
                    UserType = "Laundry"
                };
                await _context.Contacts.AddAsync(contact);

                await _context.SaveChangesAsync();

                var authResponse = await _auth.LoginLaundryAsync(new LoginData
                {
                    Username = data.Username,
                    Password = data.Password
                });

                return Ok(new
                {
                    Token = authResponse.Token,
                    Username = authResponse.Username,
                    UserId = authResponse.UserId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = ex.Message
                });
            }
        }

        [HttpPost("owner/register")]
        public async Task<IActionResult> OwnerRegister([FromBody] OwnerRegisterDTO data)
        {
            try
            {
                if (await _auth.IsUsernameTakenAsync(data.Username))
                {
                    return BadRequest(new { Error = "Username already exists" });
                }

                if (await _context.Owners.AnyAsync(o => o.Email == data.Email))
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

                var owner = new Owner
                {
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    Username = data.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(data.Password),
                    Email = data.Email,
                    Address = address
                };
                await _context.Owners.AddAsync(owner);

                var contact = new Contact
                {
                    ContactNumber = data.ContactNumber,
                    UserId = owner.OwnerId,
                    UserType = "Owner"
                };
                await _context.Contacts.AddAsync(contact);

                await _context.SaveChangesAsync();

                var authResponse = await _auth.LoginAdminAsync(new LoginData
                {
                    Username = data.Username,
                    Password = data.Password
                });

                return Ok(new
                {
                    Token = authResponse.Token,
                    Username = authResponse.Username,
                    UserId = authResponse.UserId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = ex.Message
                });
            }
        }

        [HttpPost("customer/login")]
        public async Task<IActionResult> CustomerLogin([FromBody] LoginData data)
        {
            try
            {
                var result = await _auth.LoginCustomerAsync(data);

                return Ok(new
                {
                    Token = result.Token,
                    Username = result.Username,
                    UserId = result.UserId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = ex.Message,
                    Recieved_Data = data.ToString()
                });
            }
        }

        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginData data)
        {
            try
            {
                var result = await _auth.LoginAdminAsync(data);

                return Ok(new
                {
                    Token = result.Token,
                    Username = result.Username,
                    UserId = result.UserId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = ex.Message
                });
            }
        }

        [HttpPost("laundry/login")]
        public async Task<IActionResult> LaundryLogin([FromBody] LoginData data)
        {
            try
            {
                var result = await _auth.LoginLaundryAsync(data);

                return Ok(new
                {
                    Token = result.Token,
                    Username = result.Username,
                    UserId = result.UserId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = ex.Message
                });
            }
        }

        [HttpPost("driver/login")]
        public async Task<IActionResult> DriverLogin([FromBody] LoginData data)
        {
            try
            {
                var result = await _auth.LoginDriverAsync(data);

                return Ok(new
                {
                    Token = result.Token,
                    Username = result.Username,
                    UserId = result.UserId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = ex.Message
                });
            }
        }
    }
}