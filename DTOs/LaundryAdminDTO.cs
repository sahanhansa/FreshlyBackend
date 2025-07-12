namespace FreshlyBackendNew.DTOs
{
    public class LaundryAdminDTO
    {
        public string? LaundryId { get; set; } // Nullable for input, will be set on creation
        public string? LaundryName { get; set; } // Nullable for input
        public string? Username { get; set; } // Nullable for input
        public string? Password { get; set; } // Added Password property
        public string? Email { get; set; } // Nullable for input
        public string? OwnerId { get; set; } // Nullable for input, will be parsed to Guid?
        public string? OwnerName { get; set; } // Nullable, set on retrieval
        public string? FullAddress { get; set; } // Nullable, set on retrieval
        public double AverageRating { get; set; } // Default to 0 for creation
        public int TotalOrders { get; set; } // Default to 0 for creation
        public int FeedbackCount { get; set; } // Default to 0 for creation
    }
}