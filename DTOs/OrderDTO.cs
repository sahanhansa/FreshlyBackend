namespace FreshlyBackendNew.DTOs
{
    public class OrderDTO
    {
        public Guid OrderId { get; set; }
        public string? PlacedDate { get; set; }
        public string? PlacedTime { get; set; }
        public string? PickupDate { get; set; }
        public string? PickupTime { get; set; }
        public string? CustomerFName { get; set; }
        public string? CustomerLName { get; set; }
        public string? StatusName { get; set; }

        // Customer details
        public CustomerDTO? Customer { get; set; }

        // Laundry details
        public LaundryDTO? Laundry { get; set; }

        // Status details
        public StatusDTO? Status { get; set; }

        // Order type details
        public OrderTypeDTO? OrderType { get; set; }

        // User details
        public UserDTO? User { get; set; }
    }

    public class CustomerDTO
    {
        public Guid CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public AddressDTO? Address { get; set; }
    }

    public class AddressDTO
    {
        public string? HouseNo { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? FullAddress { get; set; }
    }

    public class LaundryDTO
    {
        public Guid LaundryId { get; set; }
        public string? LaundryName { get; set; }
    }

    public class StatusDTO
    {
        public Guid StatusID { get; set; }
        public string? StatusName { get; set; }
    }

    public class OrderTypeDTO
    {
        public Guid TypeId { get; set; }
        public string? TypeName { get; set; }
    }

    public class UserDTO
    {
        public Guid UserId { get; set; }
        public string? Username { get; set; }
    }
}