namespace FreshlyBackendNew.DTOs
{
    public class LaundryRatingDTO
    {
        public Guid LaundryId { get; set; }
        public string? LaundryName { get; set; }
        public string? Email { get; set; }
        public string? LaundryImageLink { get; set; }
        public double? AverageRating { get; set; }
    }
}
