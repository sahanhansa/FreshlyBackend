using FreshlyBackendNew.Models;
using System;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Repositories.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetByEmailAsync(string email);
        Task<Customer?> GetByUsernameAsync(string username);
        Task<Customer?> GetWithAddressAsync(Guid id);
    }
}
