namespace FreshlyBackendNew.DTOs
{
    public class RequestPasswordResetDTO
    {
        public string Email { get; set; }
        public string UserType { get; set; } // Customer, Driver, etc.
    }
}
