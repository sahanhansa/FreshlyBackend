using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class TemporaryOrderDetail
    {
        public TemporaryOrderDetail()
        {
            Quantity = null;
        }

        // Composite Primary Key
        public Guid? TemporaryOrderId { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? ServiceId { get; set; }
        public Guid? GarmentTypeId { get; set; } // 🆕 Add this

        // Order Details
        public int? Quantity { get; set; }

        // Navigation Properties
        [ForeignKey("TemporaryOrderId")]
        public TemporaryOrder? TemporaryOrder { get; set; } 

        [ForeignKey("ItemId")]
        public Item? Item { get; set; } 

        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }

        [ForeignKey("GarmentTypeId")]
        public GarmentType? GarmentType { get; set; } // 🆕 Add this
    }
}
