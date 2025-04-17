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

        public required DateTime Date { get; set; }

        public required string Time { get; set; }

        public required decimal Amount { get; set; }

        public required string Method { get; set; }

        public required string Status { get; set; }
    }
}
