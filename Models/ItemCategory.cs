using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class ItemCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-incremented ID
        public int CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; }

        // Navigation Property
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
