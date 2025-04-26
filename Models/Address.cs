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
    }
}
