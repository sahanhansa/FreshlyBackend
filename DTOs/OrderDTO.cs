namespace FreshlyBackendNew.DTOs
{
    public class OrderDTO
    {
        public Guid OrderId { get; set; }
        
        public DateTime? PlacedDate { get; set; }
        public string? StatusName { get; set; }
        public string? OrderTypeName { get; set; }
        public string? CustomerFName { get; set; }
        public string? CustomerLName { get; set; }
    }
}