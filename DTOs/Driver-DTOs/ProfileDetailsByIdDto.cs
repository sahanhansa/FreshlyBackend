namespace FreshlyBackendNew.DTOs.Driver_DTOs
{
    public class ProfileDetailsByIdDto
    {
        public Guid DriverID { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? LicenseNumber { get; set; }

        public string? Email { get; set; }

        public string? ContactNumber { get; set; }

        public string? HomeAddress { get; set; }

        public string? VehicleNumber { get; set; }

        public string? Location { get; set; }
    }
}
