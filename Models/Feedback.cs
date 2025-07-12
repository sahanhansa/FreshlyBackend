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

        // Navigation Properties
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

    }
}
