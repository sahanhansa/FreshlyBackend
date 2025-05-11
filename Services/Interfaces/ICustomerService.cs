using FreshlyBackendNew.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface ICustomerService
    {
        // Retrieves all customer details
        Task<IEnumerable<CustomerDto>> GetCustomerDetailsAsync();

        // Retrieves a customer by ID
        Task<CustomerDto?> GetCustomerByIdAsync(Guid id);

        // Creates a new customer
        Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto);

        // Updates an existing customer
        Task<CustomerDto?> UpdateCustomerAsync(Guid id, CustomerDto customerDto);

        // Deletes a customer by ID
        Task<bool> DeleteCustomerAsync(Guid id);
    }
}