namespace FreshlyBackendNew.DTOs
{
    public class MarkOrderDto
    {
        public Guid OrderId { get; set; }
        public Guid? DriverId { get; set; }
        public string? Note { get; set; }
    }
}
