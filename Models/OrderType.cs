using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class OrderType
    {
        public OrderType()
        {
            TypeId = Guid.NewGuid();
        }

        [Key]
        public Guid TypeId { get; set; }

        [Required]
        public required string TypeName { get; set; }


        // Navigation property for one-to-many relationship
        public ICollection<Order> Orders { get; set; }

    }
}
