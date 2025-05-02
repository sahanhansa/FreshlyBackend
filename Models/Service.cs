using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Service
    {
        public Service()
        {
            ServiceId = Guid.NewGuid();
            OrderDetails = new List<OrderDetails>();
            LaundryItemServices = new List<LaundryItemService>();
        }

        // Primary Key
        [Key]
        public Guid ServiceId { get; set; }

        // Service Details
        public string? Name { get; set; }

        // Navigation Properties
        public ICollection<OrderDetails>? OrderDetails { get; set; }
        public ICollection<LaundryItemService>? LaundryItemServices { get; set; }
    }
}

