namespace FreshlyBackendNew.DTOs
{
    public class ItemWithServicesDTO
    {
        public Guid ItemId { get; set; }
        public string? ItemName { get; set; }
        public string? CategoryName { get; set; }
        public List<ServiceWithPriceDTO> Services { get; set; } = new List<ServiceWithPriceDTO>();
    }

    public class ServiceWithPriceDTO
    {
        public Guid ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public decimal? Price { get; set; }
    }
}
