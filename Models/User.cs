using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class User
    {
        public User()
        {
            UserId = Guid.NewGuid();
            Orders = new List<Order>();
            UserLaundries = new List<UserLaundry>();
            UserCustomers = new List<UserCustomer>();
        }

        [Key]
        public Guid UserId { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Email { get; set; }

        // Many-to-One Relationship with UserGroup
        public Guid UserGroupId { get; set; }
        public UserGroup UserGroup { get; set; }

        // One-to-Many Relationship with Order
        public ICollection<Order> Orders { get; set; }

        // Many-to-Many Relationship with Laundry
        public ICollection<UserLaundry> UserLaundries { get; set; }

        // Many-to-Many Relationship with Customer
        public ICollection<UserCustomer> UserCustomers { get; set; }
    }
}