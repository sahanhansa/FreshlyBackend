using FreshlyBackendNew.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> IsUsernameTakenAsync(string username);
        Task<AuthResponse> LoginCustomerAsync(LoginData loginData);
        Task<AuthResponse> LoginLaundryAsync(LoginData loginData);
        Task<AuthResponse> LoginDriverAsync(LoginData loginData);
        Task<AuthResponse> LoginAdminAsync(LoginData loginData);

        string GenerateJwtToken(string userId, string username, Dictionary<string, string> additionalClaims);
    }
}