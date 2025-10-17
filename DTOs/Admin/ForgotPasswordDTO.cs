using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.DTOs
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
