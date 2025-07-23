namespace FreshlyBackendNew.DTOs
{
    public class AddItemDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public List<GarmentTypeWithServicesDTO> GarmentTypes { get; set; } // Updated from Materials
    }

    public class GarmentTypeWithServicesDTO
    {
        public Guid GarmentTypeId { get; set; }
        public string GarmentTypeName { get; set; }
        public List<ServiceDTO> Services { get; set; }
    }

    public class ServiceDTO
    {
        public Guid ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public decimal? Price { get; set; }
    }

    public class AddGarmentTypeDTO
    {
        public string Name { get; set; }
        // Add other properties as needed
    }

    public class ItemWithGarmentTypesDTO
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
        public List<GarmentTypeWithServicesDTO> GarmentTypes { get; set; }
    }
}