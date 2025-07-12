namespace FreshlyBackendNew.DTOs
{
    public class FeedbackDTO
    {
        public Guid FeedbackId { get; set; } // Unique identifier for the feedback
        public string? Description { get; set; } // Textual description of the feedback
        public int? Rating { get; set; } // Rating value, e.g., 1-5
        public Guid? CustomerId { get; set; } // ID of the customer who provided the feedback
        public string? CustomerName { get; set; } // Full name of the customer, e.g., "Kumara Silva"
        public Guid? LaundryId { get; set; } // ID of the associated laundry service
        public string? LaundryName { get; set; } // Name of the laundry service, e.g., "Fresh Laundry"
    }
}