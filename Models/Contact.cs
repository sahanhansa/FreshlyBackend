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
        public string ContactNumber { get; set; }

        // Foreign keys
        public Guid? UserId { get; set; } // this include relavant customerid/laundryid/ownerid/driverids

        // Navigation properties
        [ForeignKey("UserId")]
        public Customer? Customer { get; set; }

        [ForeignKey("UserId")]
        public Laundry? Laundry { get; set; }

        [ForeignKey("UserId")]
        public Owner? Owner { get; set; }

        [ForeignKey("UserId")]
        public Driver? Driver { get; set; }

    }
}
