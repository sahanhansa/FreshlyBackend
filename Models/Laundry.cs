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
        public required string Passcode { get; set; }

        [Required]
        public required string Contact { get; set; }

        [Required]
        public required string Email { get; set; }

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
