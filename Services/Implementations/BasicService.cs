using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Services.Implementations
{
    public class BasicService : IBasicService
    {
        private readonly ApplicationDbContext _context;
        public BasicService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<LaundryDetailsWithImageDTO>> GetItemsByLaundryWithImageAsync()
        {
            try
            {
                // Fetch the basic laundry information with addresses
                var laundries = await _context.Laundries
                    .Include(l => l.Address)
                    .ToListAsync();

                var dtoList = new List<LaundryDetailsWithImageDTO>();

                foreach (var laundry in laundries)
                {
                    // Separately calculate average rating for each laundry
                    double averageRating = 0;
                    var orderIds = await _context.Orders
                        .Where(o => o.LaundryId == laundry.LaundryId)
                        .Select(o => o.OrderId)
                        .ToListAsync();

                    if (orderIds.Any())
                    {
                        var feedbackCount = await _context.Feedbacks
                            .Where(f => f.OrderId.HasValue && orderIds.Contains(f.OrderId.Value))
                            .CountAsync();

                        if (feedbackCount > 0)
                        {
                            var ratingSum = await _context.Feedbacks
                                .Where(f => f.OrderId.HasValue && orderIds.Contains(f.OrderId.Value))
                                .Select(f => f.Rating ?? 0) // Use 0 if Rating is null
                                .SumAsync();

                            averageRating = (double)ratingSum / feedbackCount;
                        }
                    }

                    dtoList.Add(new LaundryDetailsWithImageDTO
                    {
                        LaundryId = laundry.LaundryId.ToString(),
                        LaundryName = laundry.LaundryName ?? string.Empty,
                        City = laundry.Address?.City,
                        AverageRating = Math.Round(averageRating, 1)
                    });
                }

                return dtoList;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetLaundriesForCustomerAsync: {ex.Message}");
                throw; // Rethrow to be handled by the controller
            }
        }
    }
}
