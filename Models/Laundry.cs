using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class Laundry
    {
        public Laundry()
        {
            LaundryId = Guid.NewGuid(); 
        }

        // Primary Key
        [Key]
        public Guid LaundryId { get; set; }

        // Basic Information
        public string? LaundryName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string AccountStatus { get; set; } = "active";

        // Foreign Keys
        public Guid? AddressId { get; set; }
        public Guid? OwnerId { get; set; }

        // Navigation Properties
        [ForeignKey("AddressId")]
        public Address? Address { get; set; }

        [ForeignKey("OwnerId")]
        public Owner? Owner { get; set; }
        
        // Collection Navigation Property
        public ICollection<Feedback>? Feedbacks { get; set; }
        //public virtual ICollection<Contact>? Contacts { get; set; }

        public string? LaundryImageLink { get; set; }
    }
}
