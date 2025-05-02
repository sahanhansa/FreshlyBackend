using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class User
    {
        public User()
        {
            UserId = Guid.NewGuid(); 
            Orders = new List<Order>();
        }

        // Primary Key
        [Key]
        public Guid UserId { get; set; }

        // User Details
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }

        // Foreign Key
        public Guid? UserGroupId { get; set; }

        // Navigation Properties
        public UserGroup? UserGroup { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}