using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs.Order_DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AddressDTO?> GetCustomerAddressAsync(Guid customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer?.Address == null)
                return null;

            return new AddressDTO
            {
                AddressId = customer.Address.AddressId,
                HouseNo = customer.Address.HouseNo,
                Street = customer.Address.Street,
                City = customer.Address.City,
                PostalCode = customer.Address.PostalCode
            };
        }


    }
}
