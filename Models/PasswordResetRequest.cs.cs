namespace FreshlyBackendNew.Models
{
    public class PasswordResetRequest
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string UserType { get; set; } // e.g., "Customer", "Driver", etc.
    }
}
