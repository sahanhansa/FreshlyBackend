using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class Contact
    {
        public Contact()
        {
            ContactId = Guid.NewGuid(); 
        }

        // Primary Key
        [Key]
        public Guid ContactId { get; set; }

        // Address Details
        public required string ContactNumber { get; set; }

        // Foreign keys
        public Guid? UserId { get; set; } // this include relavant id- customerid/laundryid/ownerid/driverids

        // Discriminator for foreign key
        public string? UserType { get; set; } // Indicates the type of user- customer/laundry/owner/driver

    }
}
