using Microsoft.AspNetCore.Mvc;
using FreshlyBackendNew.Services.Interfaces;
using FreshlyBackendNew.DTOs;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")] // POST: api/Email/send
        public async Task<IActionResult> SendEmail([FromBody] EmailRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Use a local variable for template name
            string templateName = null;
            if (request.EmailDetails != null)
            {
                templateName = "VerificationEmailTemplate.html";
            }

            string body = request.Body;
            if (!string.IsNullOrEmpty(templateName))
            {
                body = await _emailService.GetEmailTemplateAsync(templateName);
                if (request.EmailDetails != null)
                {
                    // Replace placeholders in the template with EmailDetailsDto fields (except orderStatus)
                    body = body.Replace("{{CustomerName}}", request.EmailDetails.CustomerName ?? "")
                               .Replace("{{LaundryName}}", request.EmailDetails.LaundryName ?? "")
                               .Replace("{{OrderId}}", request.EmailDetails.OrderId.ToString())
                               .Replace("{{TotalAmount}}", request.EmailDetails.TotalAmount.ToString("F2"));

                    // Render items table or list
                    if (body.Contains("{{ItemsTable}}"))
                    {
                        var itemsHtml = "<table border='1' cellpadding='5' cellspacing='0'><tr><th>Item</th><th>Garment Type</th><th>Quantity</th><th>Price</th></tr>";
                        foreach (var item in request.EmailDetails.Items)
                        {
                            itemsHtml += $"<tr><td>{item.ItemName}</td><td>{item.GarmentTypeName}</td><td>{item.Quantity}</td><td>{item.Price:F2}</td></tr>";
                        }
                        itemsHtml += "</table>";
                        body = body.Replace("{{ItemsTable}}", itemsHtml);
                    }
                }
                else if (!string.IsNullOrEmpty(request.Body))
                {
                    body = body.Replace("{{Body}}", request.Body);
                }
            }

            await _emailService.SendEmailAsync(request.ToEmail, request.Subject, body, request.IsHtml);
            return Ok(new { message = "Email sent successfully" });
        }
    }
} 