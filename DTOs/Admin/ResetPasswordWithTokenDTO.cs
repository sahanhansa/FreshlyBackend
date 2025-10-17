using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.DTOs
{
    public class ResetPasswordWithTokenDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Token { get; set; }
        
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        public string NewPassword { get; set; }
    }
}