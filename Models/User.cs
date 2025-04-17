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

        public required string Username { get; set; }
        public required string Email { get; set; }

    }
}