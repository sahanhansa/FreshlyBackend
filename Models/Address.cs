using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Address
    {
        public Address()
        {
            AddressId = Guid.NewGuid(); 
        }

        // Primary Key
        [Key]
        public Guid AddressId { get; set; }

        // Address Details
        public string? HouseNo { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }

        // Navigation Properties
        public Customer? Customer { get; set; }
        public Laundry? Laundry { get; set; }
        public Owner? Owner { get; set; }
    }
}
