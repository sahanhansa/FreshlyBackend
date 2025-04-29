using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class OrderType
    {
        public OrderType()
        {
            TypeId = Guid.NewGuid(); 
            Orders = new List<Order>();
        }

        // Primary Key
        [Key]
        public Guid TypeId { get; set; }

        // Order Type Details
        public string? TypeName { get; set; }

        // Navigation Properties
        public ICollection<Order>? Orders { get; set; }
    }
}
