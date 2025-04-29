using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Payment
    {
        public Payment()
        {
            PaymentId = Guid.NewGuid(); 
        }

        // Primary Key
        [Key]
        public Guid PaymentId { get; set; }

        // Payment Details
        public DateTime? Date { get; set; }
        public DateTime? Time { get; set; }
        public decimal? Amount { get; set; }
        public string? Method { get; set; }
        public string? Status { get; set; }

        // Foreign Key
        public Guid? OrderId { get; set; }

        // Navigation Property
        public Order? Order { get; set; }
    }
}
