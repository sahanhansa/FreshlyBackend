namespace FreshlyBackendNew.DTOs
{
    public class VerifyResetCodeDTO
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }
        public string UserType { get; set; }
    }
}
