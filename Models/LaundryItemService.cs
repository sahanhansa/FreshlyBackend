using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [ForeignKey("LaundryId")]
        public Laundry? Laundry { get; set; } 

        [ForeignKey("ItemId")]
        public Item? Item { get; set; } 

        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }
    }
}
