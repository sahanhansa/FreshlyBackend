using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Customer
    {
        public Customer()
        {
            CustomerId = Guid.NewGuid();
            Orders = new List<Order>();
            Feedbacks = new List<Feedback>();
            CustomerLaundries = new List<CustomerLaundry>();
            UserCustomers = new List<UserCustomer>();
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

        [Required]
        public required string HouseNo { get; set; }

        [Required]
        public required string Street { get; set; }

        [Required]
        public required string City { get; set; }

        [Required]
        public required string PostalCode { get; set; }

        // One-to-Many Relationship with Order
        public ICollection<Order> Orders { get; set; }

        // One-to-Many Relationship with Feedback
        public ICollection<Feedback> Feedbacks { get; set; }

        // Many-to-Many Relationship with Laundry
        public ICollection<CustomerLaundry> CustomerLaundries { get; set; }

        // Many-to-Many Relationship with User
        public ICollection<UserCustomer> UserCustomers { get; set; }
    }
}
