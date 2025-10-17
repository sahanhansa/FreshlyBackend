using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class Driver
    {
        public Driver()
        {
            DriverId = Guid.NewGuid();
            AccountStatus = "active";
        }

        // Primary Key
        [Key]
        public Guid DriverId { get; set; }

        // Basic Information
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? LicenseNo { get; set; }

        // Foreign Keys
        public Guid? AddressId { get; set; }

        // Navigation Properties
        [ForeignKey("AddressId")]
        public Address? Address { get; set; }

        public string AccountStatus { get; set; } = "active";

        [Required]
        public string VehicleNo { get; set; }

        public string? ProfileImage { get; set; }

        //public virtual ICollection<Contact>? Contacts { get; set; }

    }
}
