using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Services.Implementations
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _context;

        public ItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ItemWithServicesDTO>> GetItemsByLaundryIdAsync(Guid laundryId)
        {
            var laundryItems = await _context.LaundryItemServices
                .Where(lis => lis.LaundryId == laundryId)
                .Select(lis => lis.ItemId)
                .Distinct()
                .ToListAsync();

            if (!laundryItems.Any())
                return new List<ItemWithServicesDTO>(); // Return empty list if no items found

            var itemsWithServices = await _context.Items
                .Where(i => laundryItems.Contains(i.ItemId))
                .Include(i => i.Category)
                .Include(i => i.LaundryItemServices)
                    .ThenInclude(lis => lis.Service)
                .ToListAsync();

            var result = itemsWithServices.Select(i => new ItemWithServicesDTO
            {
                ItemId = i.ItemId,
                ItemName = i.Name,
                CategoryName = i.Category?.CategoryName ?? "Other", // Default to "Other" if null
                Services = i.LaundryItemServices?
                    .Where(lis => lis.LaundryId == laundryId && lis.Service != null && lis.ServiceId.HasValue)
                    .Select(lis => new ServiceWithPriceDTO
                    {
                        ServiceId = lis.ServiceId.Value,
                        ServiceName = lis.Service.Name,
                        Price = lis.Price
                    })
                    .ToList() ?? new List<ServiceWithPriceDTO>()
            }).ToList();

            return result;
        }
    }
}
