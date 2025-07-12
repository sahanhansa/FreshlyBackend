namespace FreshlyBackendNew.DTOs
{
    public class DeliveryDetailsByIdDto
    {

        public Guid OrderId { get; set; }

        public string Status { get; set; }

        public string CustomerName { get; set; }
        public string Address { get; set; }
        public List<string> Contact { get; set; }
        public string LaundryName { get; set; }
        public List<OrderedItemsDto> OrderItems { get; set; }
        public string PaymenthMethod { get; set; }
    }
}
