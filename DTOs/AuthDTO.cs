namespace FreshlyBackendNew.DTOs
{
    public class LaundryOwnerRegisterDTO
    {
        public string? LaundryName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? ContactNumber1 { get; set; }
        public string? ContactNumber2 { get; set; }
        public string? StreetNumber { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? OwnerName { get; set; }
        public string? LastName { get; set; }
        public string? OwnerEmail { get; set; }
        public string? OwnerContact { get; set; }
        public string? HouseNo { get; set; }
        public IFormFile ProfileImage { get; set; }
    }

    public class CustomerRegisterDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public string? HouseNo { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public IFormFile ProfileImage { get; set; }
    }

    public class DriverRegisterDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? LicenseNo { get; set; }
        public string? ContactNumber { get; set; }
        public string? HouseNo { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
    }

    public class LoginData
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public class AuthResponse
    {
        public string? Token { get; set; }
        public string? Username { get; set; }
        public string? UserId { get; set; }
    }


}