using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class Admin
    {
        public Admin()
        {
            AdminId = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Role = "Admin";
        }

        // Primary Key
        [Key]
        public Guid AdminId { get; set; }

        // Basic Information
        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        // Admin Role (could be useful for role-based permissions)
        [StringLength(50)]
        public string? Role { get; set; } = "Admin";

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }

        // Password reset fields
        [StringLength(100)]
        public string? PasswordResetToken { get; set; }

        public DateTime? PasswordResetExpiry { get; set; }

        public string? LaundryImageLink { get; set; }
    }
}