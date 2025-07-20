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
        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid ItemId { get; set; }

        [Required]
        public Guid ServiceId { get; set; }

        [Required]
        public Guid LaundryId { get; set; }

        // Item Info
        [Required]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        public string? Reason { get; set; }

        public string? RejectedBy { get; set; }

        public DateTime RejectedAt { get; set; }

        // Navigation Properties
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        [ForeignKey("ItemId")]
        public Item? Item { get; set; }

        [ForeignKey("ServiceId")]
        public Item? Service { get; set; }

        [ForeignKey("LaundryId")]
        public Laundry? Laundry { get; set; }
    }
}
