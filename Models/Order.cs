using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class Order
    {
        public Order()
        {
            OrderId = Guid.NewGuid();
        }

        // Primary Key
        [Key]
        public Guid OrderId { get; set; }

        // Order Details
        public DateTime? PlacedAt { get; set; }
        public DateTime? PickupAt { get; set; }

        // Foreign Keys
        public Guid? LaundryId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StatusId { get; set; }

        //driver details-new
        public Guid? PickupDriverId { get; set; }
        public Guid? DeliveryDriverId { get; set; }

        // Navigation Properties
        [ForeignKey("LaundryId")]
        public Laundry? Laundry { get; set; } 

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; } 

        [ForeignKey("StatusId")]
        public Status? Status { get; set; }

        //payment details
        public string PaymentMethod { get; set; } // "COD" or "Online"
        public bool IsPaid { get; set; } = false;  // true if online payment already made


    }
}
