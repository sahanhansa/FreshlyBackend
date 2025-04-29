using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Order
    {
        public Order()
        {
            OrderId = Guid.NewGuid();
        }

        [Key]
        public Guid OrderId { get; set; }

        public DateTime? PlacedDate { get; set; }
        public DateTime? PlacedTime { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? PickupTime { get; set; }

        // Foreign keys
        public Guid? LaundryId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? TypeId { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? UserId { get; set; }

        // Navigation properties
        public Status? Status { get; set; }
        public OrderType? OrderType { get; set; }
        public User? User { get; set; }
        public Laundry? Laundry { get; set; }
        public Customer? Customer { get; set; }
        public Payment? Payment { get; set; }
        public ICollection<OrderDetails>? OrderDetails { get; set; }
    }
}
