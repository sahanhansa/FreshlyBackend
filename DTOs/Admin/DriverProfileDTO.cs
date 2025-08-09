namespace FreshlyBackendNew.DTOs.Admin
{
    public class AddressDTO
    {
        public Guid AddressId { get; set; }
        public string? HouseNo { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? FullAddress => $"{HouseNo}, {Street}, {City}, {PostalCode}";
    }

    public class DriverProfileDTO
    {
        public Guid DriverId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? LicenseNo { get; set; } // Use LicenseNo everywhere
        public Guid? AddressId { get; set; }
        public AddressDTO? Address { get; set; }
        public string AccountStatus { get; set; }
        public string? ProfileImage { get; set; } // S3 image URL
        public string? VehicleNo { get; set; } // Add VehicleNo to DTO
    }

    public class CreateDriverRequestDTO
    {
        // Driver fields
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? LicenseNo { get; set; } // Use LicenseNo everywhere
        public string? AccountStatus { get; set; } = "active";
        public string? ProfileImage { get; set; } // S3 image URL
        public string? VehicleNo { get; set; } // Add VehicleNo to DTO

        // Address fields
        public string? HouseNo { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
    }
}