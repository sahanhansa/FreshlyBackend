using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Order
    {
        public Order()
        {
            OrderId = Guid.NewGuid(); // Auto-generate GUID
        }

        [Key]
        public Guid OrderId { get; set; }

        public required DateTime Date { get; set; }
        public required string Time { get; set; }
        public required string Status { get; set; }

        // Pickup Info
        public required string PickupDate { get; set; }
        public required string PickupTime { get; set; }

        // Delivery Info
        public required string DeliveryDate { get; set; }
        public required string DeliveryTime { get; set; }
    }
}
