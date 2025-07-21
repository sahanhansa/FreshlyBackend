using Azure.Core;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace FreshlyBackendNew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PayHereService _payHereService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(PayHereService payHereService, ILogger<PaymentController> logger)
        {
            _payHereService = payHereService;
            _logger = logger;
        }

        [HttpPost("initiate")]
        public IActionResult InitiatePayment([FromBody] PayHerePaymentRequest request)
        {
            try
            {
                // Set URLs
                request.ReturnUrl = $"{Request.Scheme}://{Request.Host}/api/payment/return";
                request.CancelUrl = $"{Request.Scheme}://{Request.Host}/api/payment/cancel";
                request.NotifyUrl = "https://d106-45-121-88-32.ngrok-free.app/api/sessions/payment-webhook";

                var response = _payHereService.InitiatePayment(request);

                if (response.Success)
                {
                    var hash = _payHereService.GenerateHash(request);

                    var paymentData = new
                    {
                        merchant_id = request.MerchantId,
                        return_url = request.ReturnUrl,
                        cancel_url = request.CancelUrl,
                        notify_url = request.NotifyUrl,
                        order_id = request.OrderId,
                        items = request.ItemNumber,
                        currency = request.Currency,
                        amount = request.Amount.ToString("F2"),
                        first_name = request.FirstName,
                        last_name = request.LastName,
                        email = request.Email,
                        phone = request.Phone,
                        address = request.Address,
                        city = request.City,
                        country = request.Country,
                        hash = hash
                    };

                    return Ok(new { success = true, paymentData, paymentUrl = response.PaymentUrl });
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating payment");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("notify")]
        public IActionResult PaymentNotify([FromForm] PayHereNotification notification)
        {
            try
            {
                _logger.LogInformation("Payment notification received: {OrderId}", notification.OrderId);

                if (_payHereService.ValidateNotification(notification))
                {
                    // Update payment status in database
                    // notification.StatusCode: 2 = Success, -1 = Canceled, -2 = Failed, -3 = Chargedback

                    if (notification.StatusCode == "2")
                    {
                        // Payment successful
                        _logger.LogInformation("Payment successful for order: {OrderId}", notification.OrderId);
                        // Update order status, send confirmation emails, etc.
                    }
                    else
                    {
                        _logger.LogWarning("Payment failed/canceled for order: {OrderId}, Status: {Status}",
                            notification.OrderId, notification.StatusCode);
                    }

                    return Ok();
                }
                else
                {
                    _logger.LogWarning("Invalid payment notification received");
                    return BadRequest("Invalid notification");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment notification");
                return StatusCode(500);
            }
        }

        [HttpGet("return")]
        public IActionResult PaymentReturn([FromQuery] string order_id, [FromQuery] string payment_id)
        {
            // Redirect to frontend success page
            return Redirect($"https://yourfrontend.com/payment-success?orderId={order_id}&paymentId={payment_id}");
        }

        [HttpGet("cancel")]
        public IActionResult PaymentCancel([FromQuery] string order_id)
        {
            // Redirect to frontend cancel page
            return Redirect($"https://yourfrontend.com/payment-cancel?orderId={order_id}");
        }
    }
}
