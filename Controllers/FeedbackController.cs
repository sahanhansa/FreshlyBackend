using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpGet("get-feedbacks/{laundryId}")]
        public async Task<ActionResult<List<FeedbackDTO>>> GetFeedbacks(Guid laundryId)
        {
            try
            {
                var feedbacks = await _feedbackService.GetFeedbacksAsync(laundryId);

                if (feedbacks == null || feedbacks.Count == 0)
                {
                    return NotFound("No feedbacks found for this laundry.");
                }

                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
