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
            var customers = await _context.Customers
                .Include(c => c.Address)
                .ToListAsync();

            var customerDtos = new List<CustomerDto>();
            
            foreach (var customer in customers)
            {
                // Separately fetch contacts for this customer
                var contacts = await _context.Contacts
                    .Where(c => c.UserId == customer.CustomerId && c.UserType == "Customer")
                    .ToListAsync();
                
                customerDtos.Add(new CustomerDto
                {
                    CustomerId = customer.CustomerId,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Username = customer.Username,
                    AddressId = customer.AddressId,
                    Address = customer.Address != null ? $"{customer.Address.HouseNo}, {customer.Address.Street}, {customer.Address.City}, {customer.Address.PostalCode}" : null,
                    Contacts = contacts.Select(ct => ct.ContactNumber).ToList()
                });
            }
            
            return customerDtos;
        }

        // Retrieves a customer by ID
        public async Task<CustomerDto?> GetCustomerByIdAsync(Guid id)
        {
            var customer = await _context.Customers
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return null;
            }

            // Fetch contacts for this customer
            var contacts = await _context.Contacts
                .Where(c => c.UserId == customer.CustomerId && c.UserType == "Customer")
                .ToListAsync();

            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Username = customer.Username,
                AddressId = customer.AddressId,
                Address = customer.Address != null ? $"{customer.Address.HouseNo}, {customer.Address.Street}, {customer.Address.City}, {customer.Address.PostalCode}" : null,
                Contacts = contacts.Select(ct => ct.ContactNumber).ToList()
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
            
            // Create contacts if provided
            if (customerDto.Contacts != null && customerDto.Contacts.Any())
            {
                foreach (var contactNumber in customerDto.Contacts)
                {
                    var contact = new Contact
                    {
                        ContactNumber = contactNumber,
                        UserId = customer.CustomerId,
                        UserType = "Customer"
                    };
                    _context.Contacts.Add(contact);
                }
            }
            
            await _context.SaveChangesAsync();

            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Username = customer.Username,
                AddressId = customer.AddressId,
                Contacts = customerDto.Contacts ?? new List<string>()
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

            // Update contacts if provided
            if (customerDto.Contacts != null)
            {
                // Remove existing contacts
                var existingContacts = await _context.Contacts
                    .Where(c => c.UserId == id && c.UserType == "Customer")
                    .ToListAsync();
                
                if (existingContacts.Any())
                {
                    _context.Contacts.RemoveRange(existingContacts);
                }
                
                // Add new contacts
                foreach (var contactNumber in customerDto.Contacts)
                {
                    var contact = new Contact
                    {
                        ContactNumber = contactNumber,
                        UserId = customer.CustomerId,
                        UserType = "Customer"
                    };
                    _context.Contacts.Add(contact);
                }
            }

            await _context.SaveChangesAsync();

            return await GetCustomerByIdAsync(id); // Return the updated customer with contacts
        }

        // Deletes a customer by ID
        public async Task<bool> DeleteCustomerAsync(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return false;
            }

            // Check for dependent records - orders
            var hasOrders = await _context.Orders.AnyAsync(o => o.CustomerId == id);
            if (hasOrders)
            {
                throw new InvalidOperationException("Cannot delete customer with associated orders.");
            }

            // Check for dependent records - feedbacks
            var hasFeedbacks = await _context.Feedbacks.AnyAsync(f => f.Order.CustomerId == id);
            if (hasFeedbacks)
            {
                throw new InvalidOperationException("Cannot delete customer with associated feedback.");
            }

            // Delete related contacts
            var contacts = await _context.Contacts
                .Where(c => c.UserId == id && c.UserType == "Customer")
                .ToListAsync();
            
            if (contacts.Any())
            {
                _context.Contacts.RemoveRange(contacts);
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
