using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class Customer
    {
        public Customer()
        {
            CustomerId = Guid.NewGuid();
            Contacts = new List<Contact>();
            Orders = new List<Order>();
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
        [ForeignKey("AddressId")]
        public Address? Address { get; set; }

        // Collection navigation properties
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
    }
}