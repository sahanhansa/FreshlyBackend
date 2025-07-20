using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FreshlyBackendNew.DTOs;
using Microsoft.Extensions.Logging;

namespace FreshlyBackendNew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService auth, ILogger<AuthController> logger)
        {
            _auth = auth;
            _logger = logger;
        }

        [HttpPost("customer/login")]
        public async Task<IActionResult> CustomerLogin([FromBody] LoginData data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.Username) || string.IsNullOrEmpty(data.Password))
                {
                    return BadRequest(new { Error = "Username and password are required" });
                }

                var result = await _auth.LoginCustomerAsync(data);

                if (result == null)
                {
                    return Unauthorized(new { Error = "Invalid username or password" });
                }

                if (result.Token.StartsWith("Error") || result.Username == "Error")
                {
                    _logger.LogError($"Login error for user {data.Username}: {result.Token}");
                    return StatusCode(500, new { Error = "An error occurred during login" });
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
                _logger.LogError(ex, $"Exception during customer login: {ex.Message}");
                return StatusCode(500, new { Error = "An unexpected error occurred during login" });
            }
        }

        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginData data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.Username) || string.IsNullOrEmpty(data.Password))
                {
                    return BadRequest(new { Error = "Username and password are required" });
                }

                var result = await _auth.LoginAdminAsync(data);

                if (result == null)
                {
                    return Unauthorized(new { Error = "Invalid username or password" });
                }

                if (result.Token.StartsWith("Error") || result.Username == "Error")
                {
                    _logger.LogError($"Login error for admin {data.Username}: {result.Token}");
                    return StatusCode(500, new { Error = "An error occurred during login" });
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
                _logger.LogError(ex, $"Exception during admin login: {ex.Message}");
                return StatusCode(500, new { Error = "An unexpected error occurred during login" });
            }
        }

        [HttpPost("laundry/login")]
        public async Task<IActionResult> LaundryLogin([FromBody] LoginData data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.Username) || string.IsNullOrEmpty(data.Password))
                {
                    return BadRequest(new { Error = "Username and password are required" });
                }

                var result = await _auth.LoginLaundryAsync(data);

                if (result == null)
                {
                    return Unauthorized(new { Error = "Invalid username or password" });
                }

                if (result.Token.StartsWith("Error") || result.Username == "Error")
                {
                    _logger.LogError($"Login error for laundry {data.Username}: {result.Token}");
                    return StatusCode(500, new { Error = "An error occurred during login" });
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
                _logger.LogError(ex, $"Exception during laundry login: {ex.Message}");
                return StatusCode(500, new { Error = "An unexpected error occurred during login" });
            }
        }

        [HttpPost("driver/login")]
        public async Task<IActionResult> DriverLogin([FromBody] LoginData data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.Username) || string.IsNullOrEmpty(data.Password))
                {
                    return BadRequest(new { Error = "Username and password are required" });
                }

                var result = await _auth.LoginDriverAsync(data);

                if (result == null)
                {
                    return Unauthorized(new { Error = "Invalid username or password" });
                }

                if (result.Token.StartsWith("Error") || result.Username == "Error")
                {
                    _logger.LogError($"Login error for driver {data.Username}: {result.Token}");
                    return StatusCode(500, new { Error = "An error occurred during login" });
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
                _logger.LogError(ex, $"Exception during driver login: {ex.Message}");
                return StatusCode(500, new { Error = "An unexpected error occurred during login" });
            }
        }
    }
}
