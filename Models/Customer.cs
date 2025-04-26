using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Customer
    {
        public Customer()
        {
            CustomerId = Guid.NewGuid();
        
        }

        [Key]
        public Guid CustomerId { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        public required string Email { get; set; }
    }
}
