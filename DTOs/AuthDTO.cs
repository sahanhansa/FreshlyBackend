namespace FreshlyBackendNew.DTOs
{
    public class AuthDTO
    {

    }

    public class LoginData
    {
        public string Username { get; set; }
        public string Password { get; set; }

    }

    public class AuthResponse
    {
        public string Token { set; get; }
        public string Username { set; get; }
        public string UserId { set; get; }
    }
}
