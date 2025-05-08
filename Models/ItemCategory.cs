using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class ItemCategory
    {
        [Key]
       
        public Guid CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; }

        // Navigation Property
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
