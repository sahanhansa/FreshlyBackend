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

        // Deletes a customer by ID and moves data to DeletedCustomers table
        Task<bool> DeleteCustomerAsync(Guid id, string reason = "Deleted by admin");

        // Retrieves all deleted customers
        Task<IEnumerable<DeletedCustomerDto>> GetDeletedCustomersAsync();

        // Restores a customer from the deleted customers table
        Task<CustomerDto?> RestoreCustomerAsync(Guid id);
    }
}