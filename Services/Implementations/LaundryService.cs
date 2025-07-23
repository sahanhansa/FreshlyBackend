using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
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
            try
            {
                // Fetch the basic laundry information with addresses
                var laundries = await _context.Laundries
                    .Include(l => l.Address)
                    .OrderBy(l => l.LaundryName)
                    .ToListAsync();

                var dtoList = new List<LaundryWithAddressDTO>();

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

                    dtoList.Add(new LaundryWithAddressDTO
                    {
                        LaundryId = laundry.LaundryId.ToString(),
                        LaundryName = laundry.LaundryName ?? string.Empty,
                        City = laundry.Address?.City,
                        AverageRating = Math.Round(averageRating, 1),
                        // 🆕 Set the image link here:
                        LaundryImageLink = laundry.LaundryImageLink
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

                    string status = string.IsNullOrEmpty(laundry.AccountStatus) ? "inactive" : laundry.AccountStatus.ToLower() == "deleted" ? "deleted" : laundry.AccountStatus.ToLower() == "active" ? "active" : laundry.AccountStatus;
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
                        FeedbackCount = feedbackCount,
                        AccountStatus = status
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

            var laundry = new Laundry
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

                string status = string.IsNullOrEmpty(laundry.AccountStatus) ? "inactive" : laundry.AccountStatus.ToLower() == "deleted" ? "deleted" : laundry.AccountStatus.ToLower() == "active" ? "active" : laundry.AccountStatus;
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
                    FeedbackCount = feedbackCount,
                    AccountStatus = status
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetLaundryByIdAsync: {ex.Message}");
                throw; // Rethrow to be handled by the controller
            }
        }

        public async Task<LaundryDetailsDTO> GetLaundryDetailsAsync(Guid laundryId)
        {
            try
            {
                var laundry = await _context.Laundries
                    .Include(l => l.Address)
                    .Include(l => l.Owner)
                    .FirstOrDefaultAsync(l => l.LaundryId == laundryId);

                if (laundry == null)
                {
                    return null;
                }

                // Create full address
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

                // Create owner name
                string ownerName = string.Empty;
                if (laundry.Owner != null)
                {
                    var nameParts = new List<string>();
                    if (!string.IsNullOrEmpty(laundry.Owner.FirstName)) nameParts.Add(laundry.Owner.FirstName);
                    if (!string.IsNullOrEmpty(laundry.Owner.LastName)) nameParts.Add(laundry.Owner.LastName);

                    ownerName = string.Join(" ", nameParts);
                }

                return new LaundryDetailsDTO
                {
                    LaundryId = laundry.LaundryId.ToString(),
                    LaundryName = laundry.LaundryName,
                    Email = laundry.Email,
                    Address = fullAddress,
                    ContactNumber = "+94 71 234 5678", // This could be stored in the database
                    LogoUrl = null, // This would be stored in the database
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetLaundryDetailsAsync: {ex.Message}");
                throw;
            }
        }
        
    }
}
