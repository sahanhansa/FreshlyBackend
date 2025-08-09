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

        //should add garmenttypeid
        public Guid GarmentTypeId { get; set; }  // <-- Add this
        public int Quantity { get; set; }
    }

}
