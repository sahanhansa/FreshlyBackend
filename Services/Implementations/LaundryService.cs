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
                                              from address in addressGroup.DefaultIfEmpty()
                                              join order in _context.Orders on laundry.LaundryId equals order.LaundryId into orderGroup
                                              from order in orderGroup.DefaultIfEmpty()
                                              join feedback in _context.Feedbacks on order.OrderId equals feedback.OrderId into feedbackGroup
                                              select new
            try
                                              {
                                                  Laundry = laundry,
                                                  Address = address,
                                                  AverageRating = feedbackGroup.Any() ? feedbackGroup.Average(f => f.Rating) : 0
                                              }).ToListAsync();
                // Fetch the basic laundry information with addresses
                var laundries = await _context.Laundries
                    .Include(l => l.Address)
                    .ToListAsync();

                var dtoList = new List<LaundryWithAddressDTO>();

            // Map the data to a list of LaundryWithAddressDTO objects
            var dtoList = laundriesWithRatings.Select(l => new LaundryWithAddressDTO
                foreach (var laundry in laundries)
            {
                LaundryId = l.Laundry.LaundryId.ToString(),
                LaundryName = l.Laundry.LaundryName,
                City = l.Address?.City, 
                AverageRating = Math.Round((double)l.AverageRating, 1) // Round the average rating
            }).ToList();
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

                    dtoList.Add(new LaundryWithAddressDTO
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

        public async Task<List<LaundryAdminDTO>> GetLaundriesForAdminAsync()
        {
            try
            {
                var laundries = await _context.Laundries
                    .Include(l => l.Address)
                    .Include(l => l.Owner)
                    .ToListAsync();

                var dtoList = new List<LaundryAdminDTO>();

                foreach (var laundry in laundries)
                {
                    // Get order count
                    var totalOrders = await _context.Orders
                        .CountAsync(o => o.LaundryId == laundry.LaundryId);

                    // Get order IDs for this laundry
                    var orderIds = await _context.Orders
                        .Where(o => o.LaundryId == laundry.LaundryId)
                        .Select(o => o.OrderId)
                        .ToListAsync();

                    // Get feedback count and average rating
                    int feedbackCount = 0;
                    double averageRating = 0;

                    if (orderIds.Any())
                    {
                        feedbackCount = await _context.Feedbacks
                            .CountAsync(f => f.OrderId.HasValue && orderIds.Contains(f.OrderId.Value));

                        if (feedbackCount > 0)
                        {
                            var ratingSum = await _context.Feedbacks
                                .Where(f => f.OrderId.HasValue && orderIds.Contains(f.OrderId.Value))
                                .Select(f => f.Rating ?? 0) // Use 0 if Rating is null
                                .SumAsync();

                            averageRating = (double)ratingSum / feedbackCount;
                        }
                    }

                    // Create full address safely
                    string fullAddress = string.Empty;
                    if (laundry.Address != null)
                    {
                        var addressParts = new List<string>();
                        if (!string.IsNullOrEmpty(laundry.Address.HouseNo)) addressParts.Add(laundry.Address.HouseNo);
                        if (!string.IsNullOrEmpty(laundry.Address.Street)) addressParts.Add(laundry.Address.Street);
                        if (!string.IsNullOrEmpty(laundry.Address.City)) addressParts.Add(laundry.Address.City);
                        if (!string.IsNullOrEmpty(laundry.Address.PostalCode)) addressParts.Add(laundry.Address.PostalCode);

                        fullAddress = string.Join(", ", addressParts);
                    }

                    // Create owner name safely
                    string ownerName = string.Empty;
                    if (laundry.Owner != null)
                    {
                        var nameParts = new List<string>();
                        if (!string.IsNullOrEmpty(laundry.Owner.FirstName)) nameParts.Add(laundry.Owner.FirstName);
                        if (!string.IsNullOrEmpty(laundry.Owner.LastName)) nameParts.Add(laundry.Owner.LastName);

                        ownerName = string.Join(" ", nameParts);
                    }

                    dtoList.Add(new LaundryAdminDTO
                    {
                        LaundryId = laundry.LaundryId.ToString(),
                        LaundryName = laundry.LaundryName,
                        Username = laundry.Username,
                        Email = laundry.Email,
                        OwnerId = laundry.OwnerId?.ToString(),
                        OwnerName = string.IsNullOrEmpty(ownerName) ? null : ownerName,
                        FullAddress = string.IsNullOrEmpty(fullAddress) ? null : fullAddress,
                        AverageRating = Math.Round(averageRating, 1),
                        TotalOrders = totalOrders,
                        FeedbackCount = feedbackCount
                    });
                }

            return dtoList;
        }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetLaundriesForAdminAsync: {ex.Message}");
                throw; // Rethrow to be handled by the controller
            }
        }

        public async Task<LaundryAdminDTO> CreateLaundryAsync(LaundryAdminDTO laundryDto)
        {
            if (laundryDto == null)
            {
                throw new ArgumentNullException(nameof(laundryDto));
            }

            var laundry = new FreshlyBackendNew.Models.Laundry
            {
                LaundryName = laundryDto.LaundryName,
                Username = laundryDto.Username,
                Password = laundryDto.Password,
                Email = laundryDto.Email,
                OwnerId = !string.IsNullOrEmpty(laundryDto.OwnerId) ? Guid.Parse(laundryDto.OwnerId) : null
            };

            _context.Laundries.Add(laundry);
            await _context.SaveChangesAsync();

            // Return the created laundry with updated details
            return await GetLaundryByIdAsync(laundry.LaundryId);
        }

        public async Task<LaundryAdminDTO> GetLaundryByIdAsync(Guid id)
        {
            try
            {
                // First, get the basic laundry information
                var laundry = await _context.Laundries
                    .Include(l => l.Address)
                    .Include(l => l.Owner)
                    .FirstOrDefaultAsync(l => l.LaundryId == id);

                if (laundry == null)
                {
                    return null;
                }

                // Separately count orders and feedback
                var totalOrders = await _context.Orders
                    .CountAsync(o => o.LaundryId == id);

                // Get order IDs for this laundry
                var orderIds = await _context.Orders
                    .Where(o => o.LaundryId == id)
                    .Select(o => o.OrderId)
                    .ToListAsync();

                // Get feedback count and average rating
                int feedbackCount = 0;
                double averageRating = 0;

                if (orderIds.Any())
                {
                    feedbackCount = await _context.Feedbacks
                        .CountAsync(f => f.OrderId.HasValue && orderIds.Contains(f.OrderId.Value));

                    if (feedbackCount > 0)
                    {
                        var ratingSum = await _context.Feedbacks
                            .Where(f => f.OrderId.HasValue && orderIds.Contains(f.OrderId.Value))
                            .Select(f => f.Rating ?? 0) // Use 0 if Rating is null
                            .SumAsync();

                        averageRating = (double)ratingSum / feedbackCount;
                    }
                }

                // Create full address safely
                string fullAddress = string.Empty;
                if (laundry.Address != null)
                {
                    var addressParts = new List<string>();
                    if (!string.IsNullOrEmpty(laundry.Address.HouseNo)) addressParts.Add(laundry.Address.HouseNo);
                    if (!string.IsNullOrEmpty(laundry.Address.Street)) addressParts.Add(laundry.Address.Street);
                    if (!string.IsNullOrEmpty(laundry.Address.City)) addressParts.Add(laundry.Address.City);
                    if (!string.IsNullOrEmpty(laundry.Address.PostalCode)) addressParts.Add(laundry.Address.PostalCode);

                    fullAddress = string.Join(", ", addressParts);
                }

                // Create owner name safely
                string ownerName = string.Empty;
                if (laundry.Owner != null)
                {
                    var nameParts = new List<string>();
                    if (!string.IsNullOrEmpty(laundry.Owner.FirstName)) nameParts.Add(laundry.Owner.FirstName);
                    if (!string.IsNullOrEmpty(laundry.Owner.LastName)) nameParts.Add(laundry.Owner.LastName);

                    ownerName = string.Join(" ", nameParts);
                }

                // Create and return the DTO
                return new LaundryAdminDTO
                {
                    LaundryId = laundry.LaundryId.ToString(),
                    LaundryName = laundry.LaundryName,
                    Username = laundry.Username,
                    Email = laundry.Email,
                    OwnerId = laundry.OwnerId?.ToString(),
                    OwnerName = string.IsNullOrEmpty(ownerName) ? null : ownerName,
                    FullAddress = string.IsNullOrEmpty(fullAddress) ? null : fullAddress,
                    AverageRating = Math.Round(averageRating, 1),
                    TotalOrders = totalOrders,
                    FeedbackCount = feedbackCount
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetLaundryByIdAsync: {ex.Message}");
                throw; // Rethrow to be handled by the controller
            }
        }
    }
}
