using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class OrderItem
    {
        [Key]
        public Guid OrderItemId { get; set; } = Guid.NewGuid();

        // Foreign Key for Order
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        // Foreign Key for Item
        public Guid ItemId { get; set; }
        public Item Item { get; set; }
    }
}
