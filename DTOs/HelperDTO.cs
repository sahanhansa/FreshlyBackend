namespace FreshlyBackendNew.DTOs
{
    public class LaundryDetailsWithImageDTO
    {
        public string? LaundryId { get; set; }
        public string? LaundryName { get; set; }
        public string? City { get; set; }
        public string? LaundryImageLink { get; set; }
        public double? AverageRating { get; set; }

    }
    public class TableCountsDto
    {
        public int CustomerCount { get; set; }
        public int LaundryCount { get; set; }
        public int OrderCount { get; set; }
    }

}
