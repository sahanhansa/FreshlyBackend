using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Repositories.Implementations
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Customer?> GetByUsernameAsync(string username)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Username == username);
        }

        public async Task<Customer?> GetWithAddressAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.CustomerId == id);
        }
    }
}
