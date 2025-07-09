namespace FreshlyBackendNew.DTOs
{
    public class AddToBasketDTO
    {
        public Guid CustomerId { get; set; }
        public Guid LaundryId { get; set; }
        public List<BasketItemDTO> Items { get; set; } = new();
    }

    public class BasketItemDTO
    {
        public Guid ItemId { get; set; }
        public Guid ServiceId { get; set; }
        public int Quantity { get; set; }
    }

}
