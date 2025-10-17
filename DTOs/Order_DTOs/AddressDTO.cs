namespace FreshlyBackendNew.DTOs.Order_DTOs
{
    public class AddressDTO
    {
        public Guid AddressId { get; set; }
        public string? HouseNo { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public List<string>? ContactNumbers { get; set; }
    }
}