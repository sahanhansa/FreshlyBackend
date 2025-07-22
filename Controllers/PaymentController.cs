using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using FreshlyBackendNew.DTOs;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IConfiguration _config;

    public PaymentController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("generate-hash")]
    public IActionResult GenerateHash([FromBody] PaymentRequestDto data)
    {
        var merchantId = _config["PayHere:MerchantId"];
        var merchantSecret = _config["PayHere:MerchantSecret"];
        var amount = data.Amount.ToString("F2"); // e.g., "1000.00"
        var currency = "LKR";

        // md5(secret) first
        using var md5 = MD5.Create();
        var secretMd5Bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(merchantSecret));
        var secretMd5 = BitConverter.ToString(secretMd5Bytes).Replace("-", "").ToUpper();

        var raw = merchantId + data.OrderId + amount + currency + secretMd5;

        var rawBytes = Encoding.UTF8.GetBytes(raw);
        var hashBytes = md5.ComputeHash(rawBytes);
        var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();

        return Ok(new
        {
            MerchantId = merchantId,
            Amount = amount,
            Currency = currency,
            Hash = hash
        });
    }

    // [Optional] Payment notify_url for verification
    [HttpPost("payhere/notify")]
    public IActionResult PayHereNotify([FromForm] PayHereNotificationDto notification)
    {
        var merchantSecret = _config["PayHere:MerchantSecret"];
        var secretMd5 = CreateMd5(merchantSecret).ToUpper();

        var localMd5 = CreateMd5(
            notification.merchant_id + notification.order_id +
            notification.payhere_amount + notification.payhere_currency +
            notification.status_code + secretMd5).ToUpper();

        if (localMd5 == notification.md5sig && notification.status_code == "2")
        {
            // Payment success, update your DB
        }
        else
        {
            // Payment failed or invalid
        }

        return Ok();
    }

    private string CreateMd5(string input)
    {
        using var md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "");
    }
}
