using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Item
    {
        public Item()
        {
            ItemId = Guid.NewGuid();
            
        }

        [Key]
        public Guid ItemId { get; set; }

        [Required]
        public required string Name { get; set; }

        // Navigation property for OrderDetails
        public ICollection<OrderDetails> OrderDetails { get; set; }

        // Navigation property for LaundryItemService
        public ICollection<LaundryItemService> LaundryItemServices { get; set; }


    }
}