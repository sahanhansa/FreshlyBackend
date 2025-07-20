namespace FreshlyBackendNew.DTOs
{
    public class ItemWithServicesDTO
    {
        public Guid ItemId { get; set; }
        public string? ItemName { get; set; }
        public string? CategoryName { get; set; }
        public string? ImageUrl { get; set; }
        public List<ServiceWithPriceDTO> Services { get; set; } = new List<ServiceWithPriceDTO>();
    }

    public class ServiceWithPriceDTO
    {
        public Guid ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public decimal? Price { get; set; }
    }
    
    public class UpdateItemDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        public List<ServiceWithPriceDTO> Services { get; set; } = new List<ServiceWithPriceDTO>();
    }

}
