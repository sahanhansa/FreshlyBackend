using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Item
    {
        public Item()
        {
            ItemId = Guid.NewGuid();
            ItemServices = new List<ItemService>();
            OrderItems = new List<OrderItem>();
        }

        [Key]
        public Guid ItemId { get; set; }

        [Required]
        public required string Name { get; set; }

        // One-to-Many Relationship with Laundry
        public Guid LaundryId { get; set; }
        public Laundry Laundry { get; set; }

        // Many-to-Many Relationship with Service
        public ICollection<ItemService> ItemServices { get; set; }

        // Many-to-Many Relationship with Order
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}