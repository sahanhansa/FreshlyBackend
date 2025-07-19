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
        public string? LicensNo { get; set; }
        public Guid? AddressId { get; set; }
        public AddressDTO? Address { get; set; }
        public string AccountStatus { get; set; }
    }
}