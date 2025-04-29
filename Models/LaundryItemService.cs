using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class LaundryItemService
    {
        // Composite Primary Key
        public Guid? LaundryId { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? ServiceId { get; set; }

        // Service Details
        public decimal? Price { get; set; }

        // Navigation Properties
        public Laundry? Laundry { get; set; }
        public Item? Item { get; set; }
        public Service? Service { get; set; }
    }
}
