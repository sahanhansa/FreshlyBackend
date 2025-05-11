using Microsoft.AspNetCore.Mvc;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Services;
using FreshlyBackendNew.Data;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginData data)
    {
        string username = data.Username;
        string password = data.Password;

        try
        {
            var result = await _authService.LoginAsync(data);

        if (result == null)
        {
            return Unauthorized(new
            {
                Error = "Invalid Credentials"
            });
        }

        return Ok(new
        {
            Message = "Success",
            Result = result
        });
    }catch(Exception ex)
        {
        return BadRequest(new
        {
            Error = ex.Message
});
    }
} 
}
    