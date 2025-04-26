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

    }
}