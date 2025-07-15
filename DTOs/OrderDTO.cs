namespace FreshlyBackendNew.DTOs
{
    public class OrderDTO
    {
        public Guid OrderId { get; set; }
        public DateTime? PlacedDate { get; set; }
       
        public string StatusName { get; set; } = string.Empty;
    
        public string CustomerFName { get; set; } = string.Empty;
        
        public string CustomerLName { get; set; } = string.Empty;
    }



}