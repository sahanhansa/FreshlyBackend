namespace FreshlyBackendNew.DTOs.Driver_DTOs
{
    public class DriverEditDto
    {
        public Guid DriverID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string[]? ContactNumber { get; set; }
        public string? HouseNo { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
    }
}
