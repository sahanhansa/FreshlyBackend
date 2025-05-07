namespace FreshlyBackendNew.Models
{
    public class LoginData
    {
        public string Username { get; set; }
        public string Password { get; set; }
        
    }

    public class AuthResponse
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
    }
}