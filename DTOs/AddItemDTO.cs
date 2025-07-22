namespace FreshlyBackendNew.DTOs
{
    public class AddItemDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public List<MaterialWithServicesDTO> Materials { get; set; } // Changed from Services
    }

    public class MaterialWithServicesDTO
    {
        public Guid MaterialId { get; set; }
        public List<ServiceDTO> Services { get; set; }
    }

    public class ServiceDTO
    {
        public Guid ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public decimal? Price { get; set; }
    }
}