namespace FreshlyBackendNew.DTOs
{
    public class OrderDTO
    {
        public Guid OrderId { get; set; }
        public string? PlacedDate { get; set; }
        public string? PlacedTime { get; set; }
        public string? PickupDate { get; set; }
        public string? PickupTime { get; set; }
        public DateTime? PlacedDateTime { get; set; }
        public decimal? TotalCost { get; set; }
        public string? PaymentMethod { get; set; }
        public bool? IsPaid { get; set; }

        public CustomerDTO? Customer { get; set; }
        public LaundryDTO? Laundry { get; set; }
        public StatusDTO? Status { get; set; }
        public OrderTypeDTO? OrderType { get; set; }
        public UserDTO? User { get; set; }
    }

    public class CustomerDTO
    {
        public Guid CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string CustomerFName { get; set; } = string.Empty;
        public string CustomerLName { get; set; } = string.Empty;
        public AddressDTO? Address { get; set; }
    }

    // ✅ SINGLE AddressDTO - FIXED: Changed field to property
    public class AddressDTO
    {
        public Guid AddressId { get; set; }
        public string? HouseNo { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? FullAddress { get; set; } // ✅ FIXED: property not field
    }

    public class LaundryDTO
    {
        public Guid LaundryId { get; set; }
        public string? LaundryName { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }

    public class StatusDTO
    {
        public Guid StatusID { get; set; }
        public string? StatusName { get; set; }
        public string CustomerFName { get; set; } = string.Empty;
        public string StatusDisplayName { get; set; } = string.Empty;
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

    public class SortedOrderIdDTO
    {
        public Guid OrderId { get; set; }
    }

    public class SortedOrderIdsResponseDTO
    {
        public List<Guid> OrderIds { get; set; } = new List<Guid>();
    }

    public class PaginatedOrderResponseDTO
    {
        public List<OrderDTO> Orders { get; set; } = new List<OrderDTO>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}