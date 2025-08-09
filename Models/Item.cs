using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class Item
    {
        public Item()
        {
            ItemId = Guid.NewGuid();
        }

        // Primary Key
        [Key]
        public Guid ItemId { get; set; }

        // Item Details
        public string? Name { get; set; }
        
        public string? Description { get; set; }
        
        public string? ItemImageLink { get; set; }

        // Foreign Key
        public Guid CategoryId { get; set; }

        // Navigation Property
        [ForeignKey("CategoryId")]
        public ItemCategory? Category { get; set; }
    }
}