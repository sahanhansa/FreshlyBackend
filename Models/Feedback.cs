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
    }
}
