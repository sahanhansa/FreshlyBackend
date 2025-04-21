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

        // Many-to-One Relationship with Laundry
        public Guid LaundryId { get; set; }
        public Laundry Laundry { get; set; }

        // Many-to-One Relationship with Customer
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
