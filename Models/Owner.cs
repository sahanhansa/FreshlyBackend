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
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string OwnerName { get; set; } = string.Empty;

        [Required]
        public string Contact { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string HouseNo { get; set; } = string.Empty;

        [Required]
        public string Street { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        // One-to-One Relationship
        public Laundry Laundry { get; set; }
    }
}
