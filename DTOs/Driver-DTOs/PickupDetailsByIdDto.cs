namespace FreshlyBackendNew.DTOs
{
    public class PickupDetailsByIdDto
    {
        public Guid OrderId { get; set; }

        public string Status { get; set; }

        public string CustomerName { get; set; }
        public string Address { get; set; }
        public Guid? PickupDriverId { get; set; }
        public List<string> Contact { get; set; }
        public string LaundryName { get; set; }
        public string note { get; set; }
        public List<OrderedItemsDto> OrderItems { get; set; }
    }
}
