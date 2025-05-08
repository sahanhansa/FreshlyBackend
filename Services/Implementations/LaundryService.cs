using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Services.Implementations
{
    public class LaundryService : ILaundryService
    {
        private readonly ApplicationDbContext _context;

        public LaundryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LaundryWithAddressDTO>> GetLaundriesForCustomerAsync()
        {
            var laundriesWithRatings = await _context.Laundries
                .Include(l => l.Address) // Ensure address is included
                .Include(l => l.Feedbacks) // Include feedbacks to calculate average rating
                .Select(l => new
                {
                    Laundry = l,
                    AverageRating = l.Feedbacks.Any() ? l.Feedbacks.Average(f => f.Rating) : 0
                })
                .ToListAsync();

            var dtoList = laundriesWithRatings.Select(l => new LaundryWithAddressDTO
            {
                LaundryId = l.Laundry.LaundryId.ToString(),
                LaundryName = l.Laundry.LaundryName,
                City = l.Laundry.Address.City,
                AverageRating = Math.Round(l.AverageRating ?? 0, 1) // Round to 1 decimal place
            }).ToList();

            return dtoList;
        }
    }
}
