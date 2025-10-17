using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Repositories.Interfaces;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace FreshlyBackendNew.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            IAdminRepository adminRepository,
            IFileStorageService fileStorageService,
            ILogger<AdminService> logger)
        {
            _adminRepository = adminRepository;
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        public async Task<IEnumerable<AdminDTO>> GetAllAdminsAsync()
        {
            var admins = await _adminRepository.GetAllAsync();
            return admins.Select(MapToDTO);
        }

        public async Task<AdminDTO?> GetAdminByIdAsync(Guid adminId)
        {
            var admin = await _adminRepository.GetByIdAsync(adminId);
            return admin == null ? null : MapToDTO(admin);
        }

        public async Task<AdminDTO> CreateAdminAsync(CreateAdminRequestDTO dto, IFormFile? profileImage)
        {
            // Validation
            if (await _adminRepository.UsernameExistsAsync(dto.Username))
                throw new InvalidOperationException("Username already exists");

            if (!string.IsNullOrEmpty(dto.Email) && await _adminRepository.EmailExistsAsync(dto.Email))
                throw new InvalidOperationException("Email already exists");

            // Upload image
            string? imageUrl = null;
            if (profileImage != null && profileImage.Length > 0)
            {
                imageUrl = await _fileStorageService.UploadFileAsync(profileImage, "admin-profile-images");
            }

            // Create admin
            var admin = new Admin
            {
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Role = dto.Role ?? "Admin",
                CreatedAt = DateTime.UtcNow,
                LaundryImageLink = imageUrl
            };

            await _adminRepository.CreateAsync(admin);
            return MapToDTO(admin);
        }

        public async Task<bool> UpdateAdminAsync(Guid adminId, AdminDTO dto, Guid requestingUserId, string requestingUserRole)
        {
            var admin = await _adminRepository.GetByIdAsync(adminId);
            if (admin == null) return false;

            // Authorization check
            if (requestingUserRole != "SuperAdmin" && requestingUserId != adminId)
                throw new UnauthorizedAccessException("You don't have permission to update this admin");

            // Update username
            if (!string.IsNullOrEmpty(dto.Username) && dto.Username != admin.Username)
            {
                if (await _adminRepository.UsernameExistsAsync(dto.Username, adminId))
                    throw new InvalidOperationException("Username already exists");
                admin.Username = dto.Username;
            }

            // Update password
            if (!string.IsNullOrEmpty(dto.Password))
            {
                admin.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                admin.PasswordResetToken = null;
                admin.PasswordResetExpiry = null;
            }

            // Update other properties
            admin.FirstName = dto.FirstName ?? admin.FirstName;
            admin.LastName = dto.LastName ?? admin.LastName;
            admin.Email = dto.Email ?? admin.Email;

            // Only SuperAdmin can update roles
            if (requestingUserRole == "SuperAdmin" && !string.IsNullOrEmpty(dto.Role))
            {
                admin.Role = dto.Role;
            }

            await _adminRepository.UpdateAsync(admin);
            return true;
        }

        public async Task<bool> DeleteAdminAsync(Guid adminId)
        {
            var admin = await _adminRepository.GetByIdAsync(adminId);
            if (admin == null) return false;

            // Prevent deleting the last SuperAdmin
            if (admin.Role == "SuperAdmin")
            {
                var superAdminCount = await _adminRepository.CountSuperAdminsAsync();
                if (superAdminCount <= 1)
                    throw new InvalidOperationException("Cannot delete the last SuperAdmin");
            }

            await _adminRepository.DeleteAsync(admin);
            return true;
        }

        public async Task<string> ResetPasswordAsync(Guid adminId)
        {
            var admin = await _adminRepository.GetByIdAsync(adminId);
            if (admin == null)
                throw new KeyNotFoundException("Admin not found");

            string newPassword = GenerateSecurePassword();
            admin.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _adminRepository.UpdateAsync(admin);

            return newPassword;
        }

        public async Task<(bool success, string? token, DateTime? expiry)> ForgotPasswordAsync(string email)
        {
            var admin = await _adminRepository.GetByEmailAsync(email);
            if (admin == null)
                return (false, null, null);

            string resetToken = Guid.NewGuid().ToString("N");
            var expiry = DateTime.UtcNow.AddHours(24);

            admin.PasswordResetToken = resetToken;
            admin.PasswordResetExpiry = expiry;

            await _adminRepository.UpdateAsync(admin);
            return (true, resetToken, expiry);
        }

        public async Task<bool> ResetPasswordWithTokenAsync(string email, string token, string newPassword)
        {
            var admin = await _adminRepository.GetByEmailAsync(email);
            if (admin == null || admin.PasswordResetToken != token)
                return false;

            if (admin.PasswordResetExpiry == null || admin.PasswordResetExpiry < DateTime.UtcNow)
                throw new InvalidOperationException("Password reset token has expired");

            admin.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            admin.PasswordResetToken = null;
            admin.PasswordResetExpiry = null;

            await _adminRepository.UpdateAsync(admin);
            return true;
        }

        // Helper methods
        private static AdminDTO MapToDTO(Admin admin)
        {
            return new AdminDTO
            {
                AdminId = admin.AdminId.ToString(),
                Username = admin.Username,
                Password = null, // Never return password
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Email = admin.Email,
                Role = admin.Role,
                CreatedAt = admin.CreatedAt,
                LastLogin = admin.LastLogin,
                PasswordResetToken = admin.PasswordResetToken,
                PasswordResetExpiry = admin.PasswordResetExpiry,
                LaundryImageLink = admin.LaundryImageLink
            };
        }

        private static string GenerateSecurePassword()
        {
            // Generate a more secure 12-character password
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(12))[..12];
        }
    }
}