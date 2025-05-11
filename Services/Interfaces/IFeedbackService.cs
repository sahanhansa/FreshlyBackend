using FreshlyBackendNew.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Services
{
    public interface IFeedbackService
    {
        // Get all feedback with customer and laundry details
        Task<List<FeedbackDTO>> GetAllFeedbacksAsync();

        // Get a specific feedback by ID
        Task<FeedbackDTO> GetFeedbackByIdAsync(Guid id);

        // Create a new feedback
        Task<FeedbackDTO> CreateFeedbackAsync(FeedbackDTO feedbackDto);

        // Update an existing feedback
        Task<bool> UpdateFeedbackAsync(Guid id, FeedbackDTO feedbackDto);

        // Delete a feedback
        Task<bool> DeleteFeedbackAsync(Guid id);
    }
}