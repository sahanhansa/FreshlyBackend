using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all customer details
        public async Task<IEnumerable<CustomerDto>> GetCustomerDetailsAsync()
        {
            return await _context.Customers
                .Include(c => c.Address)
                .Include(c => c.Contacts)
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    Username = c.Username,
                    AddressId = c.AddressId,
                    Address = c.Address != null ? $"{c.Address.HouseNo}, {c.Address.Street}, {c.Address.City}, {c.Address.PostalCode}" : null,
                    Contacts = c.Contacts != null ? c.Contacts.Select(ct => ct.ContactNumber).ToList() : new List<string>()
                })
                .ToListAsync();
        }

        // Retrieves a customer by ID
        public async Task<CustomerDto?> GetCustomerByIdAsync(Guid id)
        {
            var customer = await _context.Customers
                .Include(c => c.Address)
                .Include(c => c.Contacts)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return null;
            }

            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Username = customer.Username,
                AddressId = customer.AddressId,
                Address = customer.Address != null ? $"{customer.Address.HouseNo}, {customer.Address.Street}, {customer.Address.City}, {customer.Address.PostalCode}" : null,
                Contacts = customer.Contacts != null ? customer.Contacts.Select(ct => ct.ContactNumber).ToList() : new List<string>()
            };
        }

        // Creates a new customer
        public async Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto)
        {
            if (customerDto == null)
            {
                throw new ArgumentNullException(nameof(customerDto));
            }

            var customer = new Customer
            {
                CustomerId = Guid.NewGuid(),
                FirstName = customerDto.FirstName,
                LastName = customerDto.LastName,
                Email = customerDto.Email,
                Username = customerDto.Username,
                AddressId = customerDto.AddressId
                // Note: Password should be hashed before saving (e.g., using BCrypt)
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Username = customer.Username,
                AddressId = customer.AddressId
            };
        }

        // Updates an existing customer
        public async Task<CustomerDto?> UpdateCustomerAsync(Guid id, CustomerDto customerDto)
        {
            if (customerDto == null || id != customerDto.CustomerId)
            {
                return null;
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return null;
            }

            customer.FirstName = customerDto.FirstName;
            customer.LastName = customerDto.LastName;
            customer.Email = customerDto.Email;
            customer.Username = customerDto.Username;
            customer.AddressId = customerDto.AddressId;

            await _context.SaveChangesAsync();

            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Username = customer.Username,
                AddressId = customer.AddressId
            };
        }

        // Deletes a customer by ID
        public async Task<bool> DeleteCustomerAsync(Guid id)
        {
            var customer = await _context.Customers
                .Include(c => c.Orders)
                .Include(c => c.Feedbacks)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return false;
            }

            // Check for dependent records
            if (customer.Orders != null && customer.Orders.Any())
            {
                throw new InvalidOperationException("Cannot delete customer with associated orders.");
            }

            if (customer.Feedbacks != null && customer.Feedbacks.Any())
            {
                throw new InvalidOperationException("Cannot delete customer with associated feedback.");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}