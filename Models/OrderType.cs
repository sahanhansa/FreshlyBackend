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
    }
}
