using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Address
    {
        public Address()
        {
            AddressId = Guid.NewGuid();
        }

        [Key]
        public Guid AddressId { get; set; }

        [Required]
        public required string HouseNo { get; set; }

        [Required]
        public required string Street { get; set; }

        [Required]
        public required string City { get; set; }

        [Required]
        public required string PostalCode { get; set; }

        // Navigation property for the related Customer
        public Customer Customer { get; set; }

        // Navigation property for the related Laundry
        public Laundry Laundry { get; set; }

        // Navigation property for the related Owner
        public Owner Owner { get; set; }
    }
}
