using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Laundry
    {
        public Laundry()
        {
            LaundryId = Guid.NewGuid();
            Items = new List<Item>();
            Orders = new List<Order>();
            Feedbacks = new List<Feedback>();
            CustomerLaundries = new List<CustomerLaundry>();
            UserLaundries = new List<UserLaundry>();
        }

        [Key]
        public Guid LaundryId { get; set; }

        [Required]
        public required string LaundryName { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Passcode { get; set; }

        [Required]
        public required string Contact { get; set; }

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

        // One-to-One Relationship with Owner
        public Guid OwnerId { get; set; }
        public Owner Owner { get; set; }

        // One-to-Many Relationship with Item
        public ICollection<Item> Items { get; set; }

        // One-to-Many Relationship with Order
        public ICollection<Order> Orders { get; set; }

        // One-to-Many Relationship with Feedback
        public ICollection<Feedback> Feedbacks { get; set; }

        // Many-to-Many Relationship with Customer
        public ICollection<CustomerLaundry> CustomerLaundries { get; set; }

        // Many-to-Many Relationship with User
        public ICollection<UserLaundry> UserLaundries { get; set; }
    }
}
