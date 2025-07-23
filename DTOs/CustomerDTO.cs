namespace FreshlyBackendNew.DTOs
{
    public class CustomerDto
    {
        public Guid CustomerId { get; set; } // Unique identifier for the customer
        public string? FirstName { get; set; } // Customer's first name
        public string? LastName { get; set; } // Customer's last name
        public string? Email { get; set; } // Customer's email address
        public string? Username { get; set; } // Customer's username
        public Guid? AddressId { get; set; } // ID of the associated address
        public string? Address { get; set; } // Formatted address (e.g., "123 Main St, Colombo, 10100")
        public List<string> Contacts { get; set; } = new List<string>(); // List of contact details (e.g., phone numbers)
        public string AccountStatus { get; set; } // Added for frontend status logic
        public string? ProfilePic { get; set; } // S3 profile image URL
    }
}