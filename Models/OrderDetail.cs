using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class OrderDetail
    {
        public OrderDetail()
        {
            Quantity = null;
        }

        // Composite Primary Key
        public Guid? OrderId { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? ServiceId { get; set; }
        public Guid? GarmentTypeId { get; set; } // 🆕 Add this

        // Order Details
        public int? Quantity { get; set; }

        // Navigation Properties
        [ForeignKey("OrderId")]
        public Order? Order { get; set; } 

        [ForeignKey("ItemId")]
        public Item? Item { get; set; } 

        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }

        [ForeignKey("GarmentTypeId")]
        public GarmentType? GarmentType { get; set; } // 🆕 Add this
    }
}
