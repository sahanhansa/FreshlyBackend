using System;

namespace FreshlyBackendNew.DTOs
{
    public class CreateRejectedItemDTO
    {
        public Guid? OrderId { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? ServiceId { get; set; }
        public string? ItemName { get; set; }
        public int? Quantity { get; set; }
        public string? Reason { get; set; }
        public string? RejectedBy { get; set; }
        public DateTime RejectedAt { get; set; }
    }
} 