namespace FreshlyBackendNew.DTOs
{
    public class PickupDetailsDto
    {
        public Guid OrderId { get; set; }

        public string CustomerName { get; set; }

        public Guid CustomerId { get; set; }
        public string Status { get; set; }
        public string Address { get; set; }
        public Guid? PickupDriverId { get; set; }
        public Guid? DeliveryDriverId { get; set; }
        public List<string> Contact { get; set; }

        public string LaundryName { get; set; }
    }
}
