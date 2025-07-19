using FreshlyBackendNew.Models;
using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginCustomerAsync(LoginData loginData);
        Task<AuthResponse> LoginDriverAsync(LoginData loginData);
        Task<AuthResponse> LoginAdminAsync(LoginData loginData);
        Task<AuthResponse> LoginLaundryAsync(LoginData loginData);
        string GenerateJwtToken(string userId, string username, string role = "User");
    }
}
