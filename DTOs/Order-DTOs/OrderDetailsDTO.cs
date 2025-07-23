using System;
using System.Collections.Generic;

namespace FreshlyBackendNew.DTOs.Order_DTOs
{
    public class OrderDetailsDTO
    {
        // Order information
        public Guid OrderId { get; set; }
        public string? OrderIdFormatted { get; set; } // For display like "#456124"
        public DateTime? OrderDate { get; set; }
        public string? OrderDateFormatted { get; set; } // Formatted date like "07 March 2025"
        
        // Laundry information
        public string? LaundryName { get; set; }
        public string? LaundryLocation { get; set; }
        
        // Order items
        public List<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
        
        // Order financials
        public decimal TotalAmount { get; set; }
        
        // Order status
        public string? Status { get; set; }
        
        // Pickup details
        public DateTime? PickupDate { get; set; }
        public string? PickupDateFormatted { get; set; }
        public bool ShouldShowPickupDetails { get; set; }
        
        public List<ModifiedItemDTO> ModifiedItems { get; set; } = new List<ModifiedItemDTO>();
    }
    
    public class OrderItemDTO
    {
        public string? ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Guid ItemId { get; set; }
        public Guid ServiceId { get; set; }
        public string? ServiceName { get; set; }
    }
    
    public class ModifiedItemDTO
    {
        public string? ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Guid ItemId { get; set; }
        public Guid ServiceId { get; set; }
        public string? ServiceName { get; set; }
    }
}
