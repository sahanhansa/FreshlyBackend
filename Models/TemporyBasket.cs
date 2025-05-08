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

        // Navigation Properties
        public Order? Order { get; set; }
        public Item? Item { get; set; }
        public Service? Service { get; set; }
    }
}
