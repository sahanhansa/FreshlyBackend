using System.ComponentModel.DataAnnotations;

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
        public Guid? LaundryId { get; set; }
        public Guid? CustomerId { get; set; }

        // Navigation Properties
        public Laundry? Laundry { get; set; }
        public Customer? Customer { get; set; }
    }
}
