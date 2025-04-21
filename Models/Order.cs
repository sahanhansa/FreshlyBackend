using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Order
    {
        public Order()
        {
            OrderId = Guid.NewGuid();
            OrderItems = new List<OrderItem>();
        }

        [Key]
        public Guid OrderId { get; set; }

        [Required]
        public required DateTime Date { get; set; }

        [Required]
        public required string Time { get; set; }

        [Required]
        public required string Status { get; set; }

        // Pickup Info
        [Required]
        public required string PickupDate { get; set; }

        [Required]
        public required string PickupTime { get; set; }

        // Delivery Info
        [Required]
        public required string DeliveryDate { get; set; }

        [Required]
        public required string DeliveryTime { get; set; }

        // Many-to-One Relationship with Laundry
        public Guid LaundryId { get; set; }
        public Laundry Laundry { get; set; }

        // One-to-One Relationship with Payment
        public Payment Payment { get; set; }

        // Many-to-One Relationship with Customer
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }

        // Many-to-One Relationship with User
        public Guid UserId { get; set; }
        public User User { get; set; }

        // Many-to-Many Relationship with Item
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
