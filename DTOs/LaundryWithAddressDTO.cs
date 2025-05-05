namespace FreshlyBackendNew.DTOs
{
    public class LaundryWithAddressDTO
    {
        public string LaundryId { get; set; }
        public string LaundryName { get; set; }
        public string City { get; set; }
        public double AverageRating { get; set; } // Add this property
    }
}
