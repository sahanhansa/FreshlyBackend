using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Laundry
    {
        public Laundry()
        {
            LaundryId = Guid.NewGuid();
    
        }

        [Key]
        public Guid LaundryId { get; set; }

        [Required]
        public required string LaundryName { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        public required string Email { get; set; }


        // Navigation property for one-to-many relationship
        public ICollection<Order> Orders { get; set; }

        // Navigation property for one-to-many relationship with Feedback
        public ICollection<Feedback> Feedbacks { get; set; }


        // Navigation property for one-to-many relationship with Contacts
        public ICollection<Contact> Contacts { get; set; }

        // Foreign key for Address
        public Guid AddressId { get; set; }

        // Navigation property for the related Address
        public Address Address { get; set; }

        // Foreign key for Owner
        public Guid OwnerId { get; set; }

        // Navigation property for the related Owner
        public Owner Owner { get; set; }

        // Navigation property for LaundryItemService
        public ICollection<LaundryItemService> LaundryItemServices { get; set; }


    }
}
