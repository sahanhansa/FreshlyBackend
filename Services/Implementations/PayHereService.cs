using FreshlyBackendNew.Models;
using System.Security.Cryptography;
using System.Text;

namespace FreshlyBackendNew.Services.Implementations
{
    public class PayHereService
    {
        private readonly string _merchantId;
        private readonly string _merchantSecret;
        private readonly string _baseUrl;
        private readonly IConfiguration _configuration;

        public PayHereService(IConfiguration configuration)
        {
            _configuration = configuration;
            _merchantId = _configuration["PayHere:MerchantId"];
            _merchantSecret = _configuration["PayHere:MerchantSecret"];
            _baseUrl = "https://sandbox.payhere.lk/pay/checkout"; // https://sandbox.payhere.lk or https://www.payhere.lk
        }

        public string GenerateHash(PayHerePaymentRequest request)
        {
            var hashString = $"{_merchantId}{request.OrderId}{request.Amount:F2}{request.Currency}" +
                           $"{GetMd5Hash(_merchantSecret)}";
            return GetMd5Hash(hashString).ToUpper();
        }

        public bool ValidateNotification(PayHereNotification notification)
        {
            var hashString = $"{_merchantId}{notification.OrderId}{notification.PayhereAmount}" +
                           $"{notification.PayhereCurrency}{notification.StatusCode}" +
                           $"{GetMd5Hash(_merchantSecret)}";
            var generatedHash = GetMd5Hash(hashString).ToUpper();

            return generatedHash == notification.Md5sig;
        }

        private string GetMd5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes).ToLower();
            }
        }

        public PaymentResponse InitiatePayment(PayHerePaymentRequest request)
        {
            try
            {
                var orderId = Guid.NewGuid().ToString();
                request.OrderId = orderId;

                // Store payment details in database here

                return new PaymentResponse
                {
                    Success = true,
                    Message = "Payment initiated successfully",
                    PaymentUrl = $"{_baseUrl}/pay/checkout",
                    OrderId = orderId
                };
            }
            catch (Exception ex)
            {
                return new PaymentResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
