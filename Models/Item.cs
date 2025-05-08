using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Item
    {
        public Item()
        {
            ItemId = Guid.NewGuid(); 
            OrderDetails = new List<OrderDetails>();
            LaundryItemServices = new List<LaundryItemService>();
        }

        // Primary Key
        [Key]
        public Guid ItemId { get; set; }

        // Item Details
        public string? Name { get; set; }

        // Foreign Key
        public Guid CategoryId { get; set; }

        // Navigation Properties
        public ItemCategory? Category { get; set; }
        public ICollection<OrderDetails>? OrderDetails { get; set; }
        public ICollection<LaundryItemService>? LaundryItemServices { get; set; }
    }
}