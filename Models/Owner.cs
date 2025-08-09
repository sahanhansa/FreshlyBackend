using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class Owner
    {
        public Owner()
        {
            OwnerId = Guid.NewGuid(); 
        }

        // Primary Key
        [Key]
        public Guid OwnerId { get; set; }

        // Basic Information
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        // Foreign Keys
        public Guid? AddressId { get; set; }

        // Navigation Properties
        [ForeignKey("AddressId")]
        public Address? Address { get; set; }

    }
}


