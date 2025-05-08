using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Contact
    {
        public Contact()
        {
            ContactId = Guid.NewGuid(); 
        }

        [Key]
        public Guid ContactId { get; set; }
        public string ContactNumber { get; set; }

        // Foreign keys
        public Guid? CustomerId { get; set; }
        public Guid? OwnerId { get; set; }
        public Guid? LaundryId { get; set; }

        // Navigation properties
        public Customer? Customer { get; set; }
        public Owner? Owner { get; set; }
        public Laundry? Laundry { get; set; }
    }
}
