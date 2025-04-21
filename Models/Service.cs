using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Service
    {
        public Service()
        {
            ServiceId = Guid.NewGuid();
            ItemServices = new List<ItemService>();
        }

        [Key]
        public Guid ServiceId { get; set; }

        [Required]
        public required string Name { get; set; }

        // Many-to-Many Relationship with Item
        public ICollection<ItemService> ItemServices { get; set; }
    }
}

