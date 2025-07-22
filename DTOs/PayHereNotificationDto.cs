namespace FreshlyBackendNew.DTOs
{
    public class PayHereNotificationDto
    {
        public string merchant_id { get; set; }
        public string order_id { get; set; }
        public string payment_id { get; set; }
        public string payhere_amount { get; set; }
        public string payhere_currency { get; set; }
        public string status_code { get; set; }
        public string md5sig { get; set; }
        // add other fields from PayHere if needed
    }
}
