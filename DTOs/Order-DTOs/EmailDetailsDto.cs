using System.Collections.Generic;

namespace FreshlyBackendNew.DTOs.Order_DTOs
{
    public class EmailDetailsDto
    {
        public string? CustomerEmail { get; set; }
        public string? CustomerName { get; set; }
        public Guid OrderId { get; set; }
        public Guid LaundryId { get; set; }
        public string? LaundryName { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ItemDetailsDto> Items { get; set; } = new List<ItemDetailsDto>();

        public class ItemDetailsDto
        {
            public string? ItemName { get; set; }
            public string? GarmentTypeName { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }
        // Add more fields as needed for your email template
    }
} 