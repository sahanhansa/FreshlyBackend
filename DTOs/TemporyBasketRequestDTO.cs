namespace FreshlyBackendNew.DTOs
{
    public class TemporyBasketRequestDTO
    {
        public Guid? TemporyOrderId { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? ServiceId { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
    }
}
