using System;
using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class DeletedCustomer
    {
        public DeletedCustomer()
        {
            DeletedAt = DateTime.UtcNow;
        }

        // Primary Key (using the original customer ID)
        [Key]
        public Guid CustomerId { get; set; }

        // Basic Information
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }

        // Foreign Keys
        public Guid? AddressId { get; set; }

        // Deletion Information
        public DateTime DeletedAt { get; set; }
        public string? DeletionReason { get; set; }
    }
}