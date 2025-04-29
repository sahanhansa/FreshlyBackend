using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Laundry
    {
        public Laundry()
        {
            LaundryId = Guid.NewGuid(); 
            Orders = new List<Order>();
            Feedbacks = new List<Feedback>();
            Contacts = new List<Contact>();
            LaundryItemServices = new List<LaundryItemService>();
        }

        // Primary Key
        [Key]
        public Guid LaundryId { get; set; }

        // Basic Information
        public string? LaundryName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }

        // Foreign Keys
        public Guid? AddressId { get; set; }
        public Guid? OwnerId { get; set; }

        // Navigation Properties
        public Owner? Owner { get; set; }
        public Address? Address { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Feedback>? Feedbacks { get; set; }
        public ICollection<Contact>? Contacts { get; set; }
        public ICollection<LaundryItemService>? LaundryItemServices { get; set; }
    }
}
