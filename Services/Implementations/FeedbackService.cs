using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly ApplicationDbContext _context;

        public FeedbackService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all feedback with customer and laundry details
        public async Task<List<FeedbackDTO>> GetAllFeedbacksAsync()
        {
            return await _context.Feedbacks
                .Include(f => f.Customer)
                .Include(f => f.Laundry)
                .Select(f => new FeedbackDTO
                {
                    FeedbackId = f.FeedbackId,
                    Description = f.Description,
                    Rating = f.Rating,
                    CustomerId = f.CustomerId,
                    CustomerName = f.Customer != null ? $"{f.Customer.FirstName} {f.Customer.LastName}" : null,
                    LaundryId = f.LaundryId,
                    LaundryName = f.Laundry != null ? f.Laundry.LaundryName : null
                })
                .ToListAsync();
        }

        // Get a specific feedback by ID
        public async Task<FeedbackDTO> GetFeedbackByIdAsync(Guid id)
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.Customer)
                .Include(f => f.Laundry)
                .FirstOrDefaultAsync(f => f.FeedbackId == id);

            if (feedback == null)
            {
                return null;
            }

            return new FeedbackDTO
            {
                FeedbackId = feedback.FeedbackId,
                Description = feedback.Description,
                Rating = feedback.Rating,
                CustomerId = feedback.CustomerId,
                CustomerName = feedback.Customer != null ? $"{feedback.Customer.FirstName} {feedback.Customer.LastName}" : null,
                LaundryId = feedback.LaundryId,
                LaundryName = feedback.Laundry != null ? feedback.Laundry.LaundryName : null
            };
        }

        // Create a new feedback
        public async Task<FeedbackDTO> CreateFeedbackAsync(FeedbackDTO feedbackDto)
        {
            if (feedbackDto == null)
            {
                throw new ArgumentNullException(nameof(feedbackDto));
            }

            var feedback = new Feedback
            {
                FeedbackId = Guid.NewGuid(),
                Description = feedbackDto.Description,
                Rating = feedbackDto.Rating,
                CustomerId = feedbackDto.CustomerId,
                LaundryId = feedbackDto.LaundryId
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return await GetFeedbackByIdAsync(feedback.FeedbackId);
        }

        // Update an existing feedback
        public async Task<bool> UpdateFeedbackAsync(Guid id, FeedbackDTO feedbackDto)
        {
            if (feedbackDto == null || id != feedbackDto.FeedbackId)
            {
                return false;
            }

            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return false;
            }

            feedback.Description = feedbackDto.Description;
            feedback.Rating = feedbackDto.Rating;
            feedback.CustomerId = feedbackDto.CustomerId;
            feedback.LaundryId = feedbackDto.LaundryId;

            _context.Entry(feedback).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return !await FeedbackExistsAsync(id);
            }
        }

        // Delete a feedback
        public async Task<bool> DeleteFeedbackAsync(Guid id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return false;
            }

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> FeedbackExistsAsync(Guid id)
        {
            return await _context.Feedbacks.AnyAsync(f => f.FeedbackId == id);
        }
    }
}