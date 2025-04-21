using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Payment
    {
        public Payment()
        {
            PaymentId = Guid.NewGuid();
        }

        [Key]
        public Guid PaymentId { get; set; }

        [Required]
        public required DateTime Date { get; set; }

        [Required]
        public required string Time { get; set; }

        [Required]
        public required decimal Amount { get; set; }

        [Required]
        public required string Method { get; set; }

        [Required]
        public required string Status { get; set; }

        // One-to-One Relationship with Order
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
    }
}
