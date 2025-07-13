using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class FeedbackService : IFeedbackService
{
    private readonly ApplicationDbContext _context;

    public FeedbackService(ApplicationDbContext context)
    {
        _context = context;
    }

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


    }
