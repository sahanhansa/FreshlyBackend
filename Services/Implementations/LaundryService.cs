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
            // Fetch laundries with their addresses and feedbacks using the Order table
            var laundriesWithRatings = await (from laundry in _context.Laundries
                                              join address in _context.Addresses on laundry.AddressId equals address.AddressId into addressGroup
                                              from address in addressGroup.DefaultIfEmpty() // Handle null Address
                                              join order in _context.Orders on laundry.LaundryId equals order.LaundryId
                                              join feedback in _context.Feedbacks on order.OrderId equals feedback.OrderId into feedbackGroup
                                              select new
                                              {
                                                  Laundry = laundry,
                                                  Address = address,
                                                  AverageRating = feedbackGroup.Any() ? feedbackGroup.Average(f => f.Rating) : 0
                                              }).ToListAsync();

            // Map the data to a list of LaundryWithAddressDTO objects
            var dtoList = laundriesWithRatings.Select(l => new LaundryWithAddressDTO
            {
                LaundryId = l.Laundry.LaundryId.ToString(),
                LaundryName = l.Laundry.LaundryName,
                City = l.Address?.City, 
                AverageRating = Math.Round((double)l.AverageRating, 1) // Round the average rating
            }).ToList();

            return dtoList;
        }


    }
}
