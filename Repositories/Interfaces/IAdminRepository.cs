using FreshlyBackendNew.Models;

namespace FreshlyBackendNew.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<Admin?> GetByIdAsync(Guid id);
        Task<Admin?> GetByUsernameAsync(string username);
        Task<Admin?> GetByEmailAsync(string email);  // Fixed: was "GetByEmailAsunc"
        Task<IEnumerable<Admin>> GetAllAsync();
        Task<Admin> CreateAsync(Admin admin);
        Task UpdateAsync(Admin admin);
        Task DeleteAsync(Admin admin);
        Task<bool> UsernameExistsAsync(string username, Guid? excludeAdminId = null);  // Fixed: was "UsernameExistAsync"
        Task<bool> EmailExistsAsync(string email, Guid? excludeAdminId = null);
        Task<int> CountSuperAdminsAsync();  // Fixed: was "CountSuperAdminAsync"
        Task<int> SaveChangesAsync();
    }
}
