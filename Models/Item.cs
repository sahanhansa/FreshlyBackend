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

        public required string Name { get; set; }

    }
}