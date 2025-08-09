using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.DTOs
{
    public class CustomerProfileEditDto
    {
        [Required]
        public Guid CustomerId { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; }

        // Optional password update
       

        // Address fields
        public string? HouseNo { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }

 
    }
}