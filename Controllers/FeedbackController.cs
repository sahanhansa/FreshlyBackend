using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FreshlyBackendNew.Services.Implementations;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
    
        private readonly IFeedbackService _feedbackService;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public FeedbackController(IFeedbackService feedbackService, ApplicationDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _feedbackService = feedbackService;
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        private Guid GetLaundryIdFromToken()
        {
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            return Guid.Parse(userIdClaim!);
        }

        //lasini-submit feedback
        [HttpPost("order")]
        public async Task<ActionResult<FeedbackDTO>> SubmitOrderFeedback([FromBody] SubmitFeedbackDTO dto)
        {
            if (dto == null || dto.Rating is < 1 or > 5)
                return BadRequest("Invalid feedback data.");

            var createdFeedback = await _feedbackService.SubmitOrderFeedbackAsync(dto);
            return CreatedAtAction(nameof(GetFeedback), new { id = createdFeedback.FeedbackId }, createdFeedback);
        }

        //lasini-get feedback by order id
        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<FeedbackDTO>> GetFeedbackByOrderId(Guid orderId)
        {
            var feedback = await _feedbackService.GetFeedbackByOrderIdAsync(orderId);
            if (feedback == null)
                return NotFound();
            return Ok(feedback);
        }


        //Rohansi-Get feedbacks from laundry side

        [HttpGet("get-my-feedbacks/{laundryId}")]
        public async Task<ActionResult<List<FeedbackDTO>>> GetMyFeedbacks(Guid laundryId)
        {
            try
            {
                var feedbacks = await _feedbackService.GetFeedbacksAsync(laundryId);

                if (feedbacks == null || feedbacks.Count == 0)
                    return NotFound("No feedbacks found for this laundry.");

                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message} - StackTrace: {ex.StackTrace}");
            }
        }
        

        
        // POST: api/Feedback
        [HttpPost]
        public async Task<ActionResult<FeedbackDTO>> CreateFeedback([FromBody] FeedbackDTO feedbackDto)
        {
         
            if (feedbackDto == null || (feedbackDto.Rating.HasValue && (feedbackDto.Rating < 1 || feedbackDto.Rating > 5)))
            {
                return BadRequest("Invalid feedback data.");
            }

            var createdFeedback = await _feedbackService.CreateFeedbackAsync(feedbackDto);
            return CreatedAtAction(nameof(GetFeedback), new { id = createdFeedback.FeedbackId }, createdFeedback);
        }
        
        // PUT: api/Feedback/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFeedback(Guid id, [FromBody] FeedbackDTO feedbackDto)
        {
            if (feedbackDto == null || id != feedbackDto.FeedbackId || (feedbackDto.Rating.HasValue && (feedbackDto.Rating < 1 || feedbackDto.Rating > 5)))
            {
                return BadRequest("Invalid feedback data.");
            }

            var success = await _feedbackService.UpdateFeedbackAsync(id, feedbackDto);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        // DELETE: api/Feedback/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(Guid id)
        {
            var success = await _feedbackService.DeleteFeedbackAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        // GET: api/Feedback
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeedbackDTO>>> GetFeedbacks()
        {
            var feedbacks = await _feedbackService.GetAllFeedbacksAsync();
            return Ok(feedbacks);
        }

        // GET: api/Feedback/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<FeedbackDTO>> GetFeedback(Guid id)
        {
            var feedback = await _feedbackService.GetFeedbackByIdAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }
            return Ok(feedback);
        }

        // POST: api/Feedback/SendReplyEmail
        [HttpPost("SendReplyEmail")]
        public async Task<IActionResult> SendReplyEmail([FromBody] ReplyEmailRequest request)
        {
            string? toEmail = null;
            string debugInfo = $"Request.UserId: {request.UserId}\n";

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == request.UserId);
            if (customer != null && !string.IsNullOrEmpty(customer.Email))
            {
                debugInfo += $"Customer found: Email={customer.Email}; ";
                toEmail = customer.Email;
            }
            else
            {
                debugInfo += "Customer not found; ";
                var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.DriverId == request.UserId);
                if (driver != null && !string.IsNullOrEmpty(driver.Email))
                {
                    debugInfo += $"Driver found: Email={driver.Email}; ";
                    toEmail = driver.Email;
                }
                else
                {
                    debugInfo += "Driver not found; ";
                    var laundry = await _context.Laundries.FirstOrDefaultAsync(l => l.LaundryId == request.UserId);
                    if (laundry != null && !string.IsNullOrEmpty(laundry.Email))
                    {
                        debugInfo += $"Laundry found: Email={laundry.Email}; ";
                        toEmail = laundry.Email;
                    }
                    else
                    {
                        debugInfo += "Laundry not found; ";
                    }
                }
            }

            if (string.IsNullOrEmpty(toEmail))
                return NotFound(new { error = $"User email not found for the given userId in any user table. Debug: {debugInfo}" });

            await _emailService.SendEmailAsync(toEmail, request.Subject, request.Body);
            return Ok(new { message = "Email sent successfully." });
        }

        // GET: api/Feedback/laundry-ratings
        [HttpGet("laundry-ratings")]
        public async Task<ActionResult<IEnumerable<LaundryRatingDTO>>> GetLaundryRatings()
        {
            // Get all feedbacks with non-null rating and UserId that is a customer
            var customerIds = await _context.Customers.Select(c => c.CustomerId).ToListAsync();
            var feedbacks = await _context.Feedbacks
                .Where(f => f.Rating != null && f.UserId != null && customerIds.Contains(f.UserId.Value))
                .ToListAsync();

            // Group by LaundryId and calculate average
            var grouped = feedbacks
                .GroupBy(f => f.LaundryId)
                .ToDictionary(g => g.Key, g => g.Average(f => f.Rating.Value));

            // Get all laundries
            var laundries = await _context.Laundries.ToListAsync();

            // Build result
            var result = laundries
                .Where(l => grouped.ContainsKey(l.LaundryId))
                .Select(l => new LaundryRatingDTO
                {
                    LaundryId = l.LaundryId,
                    LaundryName = l.LaundryName,
                    Email = l.Email,
                    LaundryImageLink = l.LaundryImageLink,
                    AverageRating = grouped[l.LaundryId]
                })
                .ToList();

            return Ok(result);
        }
    }

    public class ReplyEmailRequest
    {
        public Guid UserId { get; set; }
        public string UserType { get; set; } = string.Empty; // "Customer", "Driver", "Laundry"
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }

    public class LaundryRatingDTO
    {
        public Guid LaundryId { get; set; }
        public string LaundryName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LaundryImageLink { get; set; } = string.Empty;
        public double AverageRating { get; set; }
    }
}