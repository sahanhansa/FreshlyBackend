using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    }
}