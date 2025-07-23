namespace FreshlyBackendNew.DTOs
{
    public class TemporaryOrderSummaryDTO
    {
        public Guid TemporaryOrderId { get; set; }
        public Guid? LaundryId { get; set; } // Make sure this is included
        public string LaundryName { get; set; }
        public string? LaundryAddress { get; set; }
        public List<TemporaryOrderItemDTO> Items { get; set; } = new();
        public decimal TotalCost { get; set; } // For consistency with OrderDTO
    }

    public class TemporaryOrderItemDTO
    {
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public string? ItemImageUrl { get; set; }
        public Guid? ServiceId { get; set; }

        public string ServiceName { get; set; }
        public Guid? GarmentTypeId { get; set; }
        public string? GarmentTypeName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal => Price * Quantity;
    }

}
