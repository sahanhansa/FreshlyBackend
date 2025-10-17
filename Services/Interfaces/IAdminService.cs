using FreshlyBackendNew.DTOs;
using Microsoft.AspNetCore.Http;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<AdminDTO>> GetAllAdminsAsync();
        Task<AdminDTO?> GetAdminByIdAsync(Guid adminId);
        Task<AdminDTO> CreateAdminAsync(CreateAdminRequestDTO adminDTO, IFormFile? profileImage);
        Task<bool> UpdateAdminAsync(Guid adminId, AdminDTO adminDTO, Guid requestingUserId, string requestingUserRole);
        Task<bool> DeleteAdminAsync(Guid adminId);
        Task<string> ResetPasswordAsync(Guid adminId);
        Task<(bool success, string? token, DateTime? expiry)> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordWithTokenAsync(string email, string token, string newPassword);
    }
}
