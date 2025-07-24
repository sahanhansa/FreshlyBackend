namespace FreshlyBackendNew.DTOs
{
    public class CustomerOrderDTO
    {
        public int OrderId { get; set; }
        public string CustomerId { get; set; }
        public string OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
    }
}
