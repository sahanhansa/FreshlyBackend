
    // Models/PayHereModels.cs
    using System.ComponentModel.DataAnnotations;

    namespace FreshlyBackendNew.Models
    {
        public class PayHerePaymentRequest
        {
            public string MerchantId { get; set; }
            public string ReturnUrl { get; set; }
            public string CancelUrl { get; set; }
            public string NotifyUrl { get; set; }
            public string OrderId { get; set; }
            public string ItemNumber { get; set; }
            public decimal Amount { get; set; }
            public string Currency { get; set; } = "LKR";
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string Country { get; set; } = "Sri Lanka";
        }

        public class PayHereNotification
        {
            public string MerchantId { get; set; }
            public string OrderId { get; set; }
            public string PayhereAmount { get; set; }
            public string PayhereCurrency { get; set; }
            public string StatusCode { get; set; }
            public string Md5sig { get; set; }
            public string Method { get; set; }
            public string StatusMessage { get; set; }
            public string CardHolderName { get; set; }
            public string CardNo { get; set; }
        }

        public class PaymentResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string PaymentUrl { get; set; }
            public string OrderId { get; set; }
        }
    }

