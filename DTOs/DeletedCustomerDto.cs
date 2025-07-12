namespace FreshlyBackendNew.DTOs
{
    public class DeletedCustomerDto
    {
        public Guid CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public DateTime DeletedAt { get; set; }
        public string? DeletionReason { get; set; }
    }
}