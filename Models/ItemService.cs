using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class ItemService
    {
        [Key]
        public Guid ItemServiceId { get; set; } = Guid.NewGuid();

        // Foreign Key for Item
        public Guid ItemId { get; set; }
        public Item Item { get; set; }

        // Foreign Key for Service
        public Guid ServiceId { get; set; }
        public Service Service { get; set; }
    }
}
