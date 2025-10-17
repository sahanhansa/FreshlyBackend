using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Repositories.Implementations
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Admin?> GetByIdAsync(Guid id)
        {
            return await _context.Admins.FindAsync(id);
        }

        public async Task<Admin?> GetByUsernameAsync(string username)
        {
            return await _context.Admins
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Username == username);
        }

        public async Task<Admin?> GetByEmailAsync(string email)
        {
            return await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            return await _context.Admins
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Admin> CreateAsync(Admin admin)
        {
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
            return admin;
        }

        public async Task UpdateAsync(Admin admin)
        {
            _context.Entry(admin).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Admin admin)
        {
            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UsernameExistsAsync(string username, Guid? excludeAdminId = null)
        {
            return await _context.Admins
                .AsNoTracking()
                .AnyAsync(a => a.Username == username && 
                              (excludeAdminId == null || a.AdminId != excludeAdminId));
        }

        public async Task<bool> EmailExistsAsync(string email, Guid? excludeAdminId = null)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            return await _context.Admins
                .AsNoTracking()
                .AnyAsync(a => a.Email == email && 
                              (excludeAdminId == null || a.AdminId != excludeAdminId));
        }

        public async Task<int> CountSuperAdminsAsync()
        {
            return await _context.Admins
                .AsNoTracking()
                .CountAsync(a => a.Role == "SuperAdmin");
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
