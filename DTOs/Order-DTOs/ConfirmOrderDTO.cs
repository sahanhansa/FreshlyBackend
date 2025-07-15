using System;

namespace FreshlyBackendNew.DTOs
{
    public class ConfirmOrderDTO
    {
        public Guid TemporaryOrderId { get; set; }
        public DateTime PickupAt { get; set; }
        public AddressDTO? Address { get; set; } // For updating existing address
    }

    public class AddressDTO
    {
        public Guid AddressId { get; set; }
        public string? HouseNo { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
    }
}