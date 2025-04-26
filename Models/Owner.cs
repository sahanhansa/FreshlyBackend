using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Owner
    {
        public Owner()
        {
            OwnerId = Guid.NewGuid(); // Auto-generate ID
        }

        [Key]
        public Guid OwnerId { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

       [Required]
        public required string Email { get; set; }


    }
}
