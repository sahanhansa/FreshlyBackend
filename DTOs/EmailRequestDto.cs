using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.DTOs
{
    public class EmailRequestDto
    {
        [Required]
        public string ToEmail { get; set; }
        [Required]
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; } = false;
        public Order_DTOs.EmailDetailsDto? EmailDetails { get; set; }
    }
} 