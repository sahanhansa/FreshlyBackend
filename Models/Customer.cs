using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Customer
    {
        public Customer()
        {
            CustomerId = Guid.NewGuid();
            Orders = new List<Order>();
            Contacts = new List<Contact>();
            Feedbacks = new List<Feedback>();
        }

        // Primary Key
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

        // Navigation Properties
        public Address? Address { get; set; }
        public ICollection<Contact>? Contacts { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Feedback>? Feedbacks { get; set; }
    }
}