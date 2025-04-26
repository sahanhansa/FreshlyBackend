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

        // Navigation property for the related Laundry
        public Laundry Laundry { get; set; }

        // Foreign key for Address
        public Guid AddressId { get; set; }

        // Navigation property for the related Address
        public Address Address { get; set; }
    }
}
