using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class LaundryItemService
    {
        // Composite Primary Key: LaundryId, ItemId, ServiceId
        public Guid LaundryId { get; set; }
        public Guid ItemId { get; set; }
        public Guid ServiceId { get; set; }

        [Required]
        public decimal Price { get; set; }

        // Navigation property for Laundry
        public Laundry Laundry { get; set; }

        // Navigation property for Item
        public Item Item { get; set; }

        // Navigation property for Service
        public Service Service { get; set; }
    }
}
