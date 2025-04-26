using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class User
    {
        public User()
        {
            UserId = Guid.NewGuid();
           
        }

        [Key]
        public Guid UserId { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        public required string Email { get; set; }

        // Foreign key for UserGroup
        public Guid UserGroupId { get; set; }

        // Navigation property for the related UserGroup
        public UserGroup UserGroup { get; set; }


        // Navigation property for one-to-many relationship
        public ICollection<Order> Orders { get; set; }


    }
}