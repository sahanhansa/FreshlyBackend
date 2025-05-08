namespace FreshlyBackendNew.Models
{
    public class TemporyBasket
    {
        public TemporyBasket()
        {
            Quantity = null; // Initialize Quantity as null
        }

        // Composite Primary Key
        public Guid? TemporyOrderId { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? ServiceId { get; set; }

        // Order Details
        public int? Quantity { get; set; }
        public decimal? Price { get; set; } // Price of the service for the item
        public decimal? TotalPrice { get; set; } // Pre-calculated total price (Price * Quantity)

        // Display Details
        public string? ItemName { get; set; } // Name of the item
        public string? ServiceName { get; set; } // Name of the service
        public string? Material { get; set; } // Material of the item
        public string? ImageUrl { get; set; } // URL of the item's image

        // Foreign Keys
        public Guid? LaundryId { get; set; } // Associated laundry
        public Guid? UserId { get; set; } // Associated user (optional)

        // Navigation Properties
        //public Order? Order { get; set; }
        //public Item? Item { get; set; }
        //public Service? Service { get; set; }
        //public Laundry? Laundry { get; set; }
        //public User? User { get; set; }
    }
}