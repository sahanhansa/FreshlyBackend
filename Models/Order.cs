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

        [Required]
        public required DateTime PlacedDate { get; set; }

        [Required]
        public required string PlacedTime { get; set; }

        [Required]
        public required string PickupDate { get; set; }

        [Required]
        public required string PickupTime { get; set; }

        [Required]
        public int Total { get; set; }

        // Foreign key for Status
        public Guid StatusId { get; set; }

        // Navigation property for the related Status
        public Status Status { get; set; }

        // Foreign key for OrderType
        public Guid TypeId { get; set; }

        // Navigation property for the related OrderType
        public OrderType OrderType { get; set; }

        // Foreign key for User
        public Guid UserId { get; set; }

        // Navigation property for the related User
        public User User { get; set; }

        // Foreign key for Laundry
        public Guid LaundryId { get; set; }

        // Navigation property for the related Laundry
        public Laundry Laundry { get; set; }

        // Foreign key for Customer
        public Guid CustomerId { get; set; }

        // Navigation property for the related Customer
        public Customer Customer { get; set; }

        // Navigation property for the related Payment
        public Payment Payment { get; set; }

        // Navigation property for OrderDetails
        public ICollection<OrderDetails> OrderDetails { get; set; }


    }
}
