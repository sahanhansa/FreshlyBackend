namespace FreshlyBackendNew.DTOs
{
    public class TemporyBasketResponseDTO
    {
        public Guid? TemporyOrderId { get; set; }
        public Guid? ItemId { get; set; }
        public string? ItemName { get; set; }
        public string? ServiceName { get; set; }
        public string? Material { get; set; }
        public string? ImageUrl { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
