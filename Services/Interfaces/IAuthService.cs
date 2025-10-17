using FreshlyBackendNew.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FreshlyBackendNew.Data;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> IsUsernameTakenAsync(string username);
        
        // New unified login method
        Task<AuthResponse> LoginAsync(string userType, LoginData data);
        
        // Keep backward compatibility - these will call LoginAsync internally
        Task<AuthResponse> LoginCustomerAsync(LoginData loginData);
        Task<AuthResponse> LoginDriverAsync(LoginData loginData);
        Task<AuthResponse> LoginLaundryAsync(LoginData loginData);
        Task<AuthResponse> LoginAdminAsync(LoginData loginData);
        
        Task EnsureDefaultAdminExistsAsync();
        Task<bool> EditCustomerProfileAsync(CustomerProfileEditDto editDto);
        Task<bool> DeleteCustomerProfileAsync(Guid customerId);
        string GenerateJwtToken(string userId, string username, Dictionary<string, string> additionalClaims);
    }
}