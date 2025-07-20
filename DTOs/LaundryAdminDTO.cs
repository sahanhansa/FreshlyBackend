namespace FreshlyBackendNew.DTOs
{
    public class LaundryAdminDTO
    {
        public string? LaundryId { get; set; }
        public string? LaundryName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? OwnerId { get; set; }
        public string? OwnerName { get; set; }
        public string? FullAddress { get; set; }
        public double AverageRating { get; set; }
        public int TotalOrders { get; set; }
        public int FeedbackCount { get; set; }
        public string? AccountStatus { get; set; }
    }

    public class CreateLaundryAccountDTO
    {
        // Owner fields
        public string? OwnerFirstName { get; set; }
        public string? OwnerLastName { get; set; }
        public string? OwnerEmail { get; set; }
        public string? OwnerPassword { get; set; }
        public string? OwnerUsername { get; set; }

        // Address fields
        public string? HouseNo { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }

        // Laundry fields
        public string? LaundryName { get; set; }
        public string? LaundryUsername { get; set; }
        public string? LaundryPassword { get; set; }
        public string? LaundryEmail { get; set; }
    }
}