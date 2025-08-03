namespace FreshlyBackendNew.DTOs.Driver_DTOs
{
    public class DriverReportDto
    {
        public int TotalPickups { get; set; }
        public int TotalDelivery { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedPickups { get; set; }
        public int CompletedDelivery { get; set; }
        public string MostEngagedLaundryName { get; set; }
        public Guid MostEngagedLaundryId { get; set; }
        public Guid MostEngagedCustomerId { get; set; }
        public string MostEngagedCustomerName { get; set; }

    }
}
