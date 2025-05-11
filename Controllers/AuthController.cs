using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FreshlyBackendNew.DTOs;
namespace FreshlyBackendNew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("customer/login")]
        public async Task<IActionResult> CustomerLogin([FromForm] LoginData data)
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
            catch(Exception ex)
            {
                return BadRequest(new
                {
                    Error = ex.Message
                });
            }
            
        }


        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin([FromForm] LoginData data)
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
        public async Task<IActionResult> LaundryLogin([FromForm] LoginData data)
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
        public async Task<IActionResult> DriverLogin([FromForm] LoginData data)
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
