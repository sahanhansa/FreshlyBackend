using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class OrderDetails
    {
        // Composite Primary Key: OrderId, ItemId, ServiceId
        public Guid OrderId { get; set; }
        public Guid ItemId { get; set; }
        public Guid ServiceId { get; set; }

        [Required]
        public int Quantity { get; set; }

        // Navigation property for Order
        public Order Order { get; set; }

        // Navigation property for Item
        public Item Item { get; set; }

        // Navigation property for Service
        public Service Service { get; set; }
    }
}
