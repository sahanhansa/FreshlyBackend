namespace FreshlyBackendNew.DTOs
{
    public class AdminDTO
    {
        public string? AdminId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; } // Only used for creation/update operations
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetExpiry { get; set; }
        public string? LaundryImageLink { get; set; } // Add this property for profile image
    }
}