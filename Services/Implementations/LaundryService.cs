using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Services.Implementations
{
    // Implementation of the ILaundryService interface.
    public class LaundryService : ILaundryService
    {
        private readonly ApplicationDbContext _context;

        public LaundryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LaundryWithAddressDTO>> GetLaundriesForCustomerAsync()
        {
            // Fetch laundries from the database, including their addresses and feedbacks
            var laundriesWithRatings = await _context.Laundries
                .Include(l => l.Address) 
                .Include(l => l.Feedbacks) 
                .Select(l => new
                {
                    Laundry = l,
                    AverageRating = l.Feedbacks.Any() ? l.Feedbacks.Average(f => f.Rating) : 0
                })
                .ToListAsync();

            // Map the data to a list of LaundryWithAddressDTO objects
            var dtoList = laundriesWithRatings.Select(l => new LaundryWithAddressDTO
            {
                LaundryId = l.Laundry.LaundryId.ToString(),
                LaundryName = l.Laundry.LaundryName,
                City = l.Laundry.Address.City,
                AverageRating = Math.Round(l.AverageRating ?? 0, 1) 
            }).ToList();

            return dtoList;
        }
    }
}
