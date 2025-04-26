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

    }
}
