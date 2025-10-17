using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.DTOs
{
    public class PasswordResetDTO
    {
        [Required]
        public string AdminId { get; set; }
    }
}
