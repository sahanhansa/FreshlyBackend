using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Customer
    {
        public Customer()
        {
            CustomerId = Guid.NewGuid();
        
        }

        [Key]
        public Guid CustomerId { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        public required string Email { get; set; }


        // Navigation property for one-to-many relationship
        public ICollection<Order> Orders { get; set; }

        // Foreign key for Address
        public Guid AddressId { get; set; }

        // Navigation property for the related Address
        public Address Address { get; set; }

        // Foreign key for Contact
        public Guid ContactId { get; set; }

        // Navigation property for the related Contacts
        public ICollection<Contact> Contacts { get; set; }


        // Navigation property for one-to-many relationship with Feedback
        public ICollection<Feedback> Feedbacks { get; set; }


    }
}
