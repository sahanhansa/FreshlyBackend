using System;

namespace FreshlyBackendNew.DTOs
{
    public class RejectedItemDTO
    {
        public Guid RejectedItemId { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? ServiceId { get; set; }
        public Guid? LaundryId { get; set; }
        public Guid? StatusId { get; set; }

        public string? ItemName { get; set; }
        public int? Quantity { get; set; }
        public string? Reason { get; set; }
        public string? RejectedBy { get; set; }
        public DateTime RejectedAt { get; set; }
    }
    public class RejectedItemWithGarmentDTO
    {
        public Guid RejectedItemId { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? ServiceId { get; set; }
        public Guid? LaundryId { get; set; }
        public Guid? GarmentTypeId { get; set; }
        public string? GarmentTypeName { get; set; }
        public string? ItemName { get; set; }
        public int? Quantity { get; set; }
        public string? Reason { get; set; }
        public string? RejectedBy { get; set; }
        public DateTime RejectedAt { get; set; }
        
        public Guid? StatusId { get; set; }
    }

}