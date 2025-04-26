using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Contact
    {
        public Guid ContactId { get; set; }

        [Required]
        public required string ContactNumber { get; set; }

        // Foreign key for Customer
        public Guid CustomerId { get; set; }

        // Navigation property for the related Customer
        public Customer Customer { get; set; }

        // Foreign key for Laundry
        public Guid LaundryId { get; set; }

        // Navigation property for the related Laundry
        public Laundry Laundry { get; set; }
    }
}
