using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using FreshlyBackendNew.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class FeedbackService : IFeedbackService
{
    private readonly ApplicationDbContext _context;

    public FeedbackService(ApplicationDbContext context)
    {
        _context = context;
    }

   //Rohansi-Get Feddbacks from laundry side
    public async Task<List<FeedbackDTO>> GetFeedbacksAsync(Guid laundryId)
    {
        return await _context.Feedbacks
            .Include(f => f.Order)
            .ThenInclude(o => o.Customer) // Include customer data
            .Where(f => f.Order != null && f.Order.LaundryId == laundryId)
            .Select(f => new FeedbackDTO
            {
                Description = f.Description ?? string.Empty,
                Rating = f.Rating ?? 0,
                CustomerFName= f.Order.Customer.FirstName ?? "Unknown" ,
                CustomerLName = f.Order.Customer.LastName ?? "Unknown" 
            })
            .ToListAsync();
    }
        // Get all feedback with customer and laundry details
        public async Task<List<FeedbackDTO>> GetAllFeedbacksAsync()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.Order)
                .ThenInclude(o => o.Customer)
                .Include(f => f.Laundry)
                .ToListAsync();

            var result = new List<FeedbackDTO>();
            foreach (var f in feedbacks)
            {
                var dto = new FeedbackDTO
                {
                    FeedbackId = f.FeedbackId,
                    Description = f.Description,
                    Rating = f.Rating,
                    LaundryId = f.LaundryId
                };

                // Get customer info from order if available
                if (f.Order != null && f.Order.Customer != null)
                {
                    dto.CustomerId = f.Order.CustomerId;
                    dto.CustomerName = $"{f.Order.Customer.FirstName} {f.Order.Customer.LastName}";
                }

                // Get laundry name if available
                if (f.Laundry != null)
                {
                    dto.LaundryName = f.Laundry.LaundryName;
                }

                result.Add(dto);
            }

            return result;
        }

        // Get a specific feedback by ID
        public async Task<FeedbackDTO> GetFeedbackByIdAsync(Guid id)
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.Order)
                .ThenInclude(o => o.Customer)
                .Include(f => f.Laundry)
                .FirstOrDefaultAsync(f => f.FeedbackId == id);

            if (feedback == null)
            {
                return null;
            }

            var dto = new FeedbackDTO
            {
                FeedbackId = feedback.FeedbackId,
                Description = feedback.Description,
                Rating = feedback.Rating,
                LaundryId = feedback.LaundryId
            };

            // Get customer info from order if available
            if (feedback.Order != null && feedback.Order.Customer != null)
            {
                dto.CustomerId = feedback.Order.CustomerId;
                dto.CustomerName = $"{feedback.Order.Customer.FirstName} {feedback.Order.Customer.LastName}";
            }

            // Get laundry name if available
            if (feedback.Laundry != null)
            {
                dto.LaundryName = feedback.Laundry.LaundryName;
            }

            return dto;
        }

        // Create a new feedback
        public async Task<FeedbackDTO> CreateFeedbackAsync(FeedbackDTO feedbackDto)
        {
            if (feedbackDto == null)
            {
                throw new ArgumentNullException(nameof(feedbackDto));
            }

            // Find the Order associated with the Customer
            Guid? orderId = null;
            if (feedbackDto.CustomerId.HasValue)
            {
                var order = await _context.Orders
                    .Where(o => o.CustomerId == feedbackDto.CustomerId && o.LaundryId == feedbackDto.LaundryId)
                    .OrderByDescending(o => o.PlacedAt)
                    .FirstOrDefaultAsync();
                
                if (order != null)
                {
                    orderId = order.OrderId;
                }
            }

            var feedback = new Feedback
            {
                FeedbackId = Guid.NewGuid(),
                Description = feedbackDto.Description,
                Rating = feedbackDto.Rating,
                OrderId = orderId,
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

            // Find the Order associated with the Customer if CustomerId is provided
            if (feedbackDto.CustomerId.HasValue)
            {
                var order = await _context.Orders
                    .Where(o => o.CustomerId == feedbackDto.CustomerId && o.LaundryId == feedbackDto.LaundryId)
                    .OrderByDescending(o => o.PlacedAt)
                    .FirstOrDefaultAsync();
                
                if (order != null)
                {
                    feedback.OrderId = order.OrderId;
                }
            }

            feedback.Description = feedbackDto.Description;
            feedback.Rating = feedbackDto.Rating;
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
