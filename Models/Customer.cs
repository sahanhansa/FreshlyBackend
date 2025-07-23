using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class Customer
    {
        public Customer()
        {
            CustomerId = Guid.NewGuid();
            //Contacts = new List<Contact>();
        }

        [Key]
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string AccountStatus { get; set; } = "active";
        public string? CustomerImageLink { get; set; }
        public Guid? AddressId { get; set; }

        // Navigation PropertiesgjdF
        [ForeignKey("AddressId")]
        public Address? Address { get; set; }
        public string? CustomerImageLink { get; set; }

    }
}