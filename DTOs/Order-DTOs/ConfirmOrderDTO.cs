using System;

namespace FreshlyBackendNew.DTOs
{
    public class ConfirmOrderDTO
    {
        public Guid TemporaryOrderId { get; set; }
        public DateTime PickupAt { get; set; }
        public UpdatedAddressDTO? Address { get; set; } // For updating existing address
        public List<ContactDTO>? Contacts { get; set; } // Add this line

    }

    public class UpdatedAddressDTO
    {
        public Guid AddressId { get; set; }
        public string? HouseNo { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
    }
    public class ContactDTO
    {
        public Guid? ContactId { get; set; } // null for new contacts
        public string ContactNumber { get; set; }
        public bool IsDeleted { get; set; } // true if the contact should be deleted
    }
}