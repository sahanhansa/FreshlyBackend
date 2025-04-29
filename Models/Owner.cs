using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Owner
    {
        public Owner()
        {
            OwnerId = Guid.NewGuid(); 
            Contacts = new List<Contact>();
        }

        // Primary Key
        [Key]
        public Guid OwnerId { get; set; }

        // Basic Information
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }

        // Foreign Keys
        public Guid? AddressId { get; set; }

        // Navigation Properties
        public Address? Address { get; set; }
        public Laundry? Laundry { get; set; }
        public ICollection<Contact>? Contacts { get; set; }
        
    }
}


