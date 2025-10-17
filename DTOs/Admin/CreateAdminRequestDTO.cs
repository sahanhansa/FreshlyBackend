using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.DTOs
{
    public class CreateAdminRequestDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? Role { get; set; }
    }
}