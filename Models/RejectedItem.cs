using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class RejectedItem
    {
        public RejectedItem()
        {
            RejectedItemId = Guid.NewGuid();
            RejectedAt = DateTime.UtcNow;
        }

        // Primary Key
        [Key]
        public Guid RejectedItemId { get; set; }

        // Foreign Keys
        
        public Guid? OrderId { get; set; }

        
        public Guid? ItemId { get; set; }

        
        public Guid? ServiceId { get; set; }

        
        public Guid? LaundryId { get; set; }
        public Guid? GarmentTypeId { get; set; }

        // Item Info

        public string? ItemName { get; set; } = string.Empty;

        
        public int? Quantity { get; set; }

        public string? Reason { get; set; }

        public string? RejectedBy { get; set; }

        public DateTime RejectedAt { get; set; }

        // Navigation Properties
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        [ForeignKey("ItemId")]
        public Item? Item { get; set; }

        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }

        [ForeignKey("LaundryId")]
        public Laundry? Laundry { get; set; }

        [ForeignKey("GarmentTypeId")]
        public GarmentType? GarmentType { get; set; }
    }
}
