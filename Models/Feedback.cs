using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Feedback
    {
        public Feedback()
        {
            FeedbackId = Guid.NewGuid();
        }

        [Key]
        public Guid FeedbackId { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public required int Rating { get; set; }

        // Foreign key for Laundry
        public Guid LaundryId { get; set; }

        // Navigation property for the related Laundry
        public Laundry Laundry { get; set; }

        // Foreign key for Customer
        public Guid CustomerId { get; set; }

        // Navigation property for the related Customer
        public Customer Customer { get; set; }
    }
}
