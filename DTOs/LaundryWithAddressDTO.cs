namespace FreshlyBackendNew.DTOs
{
    public class LaundryWithAddressDTO
    {
        public string LaundryId { get; set; } = string.Empty;
        public string LaundryName { get; set; } = string.Empty;
        public string? City { get; set; }
        public double AverageRating { get; set; }
        // 🆕 Add this line:
        public string? LaundryImageLink { get; set; }
    }
}
