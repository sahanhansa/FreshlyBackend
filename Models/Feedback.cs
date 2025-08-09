using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class Feedback
    {
        public Feedback()
        {
            FeedbackId = Guid.NewGuid();
        }

        // Primary Key
        [Key]
        public Guid FeedbackId { get; set; }

        // Feedback Details
        public string? Description { get; set; }
        public int? Rating { get; set; }

        // Foreign Keys
        public Guid? OrderId { get; set; }
        public Guid? LaundryId { get; set; }

        // Add UserId to identify which user issued the feedback
        public Guid? UserId { get; set; }

        // Navigation Properties
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
        
        [ForeignKey("LaundryId")]
        public Laundry? Laundry { get; set; }

        // Identifies who filed the feedback: "Customer", "Driver", or "Laundry"
        public string? SubmittedByType { get; set; }
        public string? InquiryType { get; set; }

    }
}
