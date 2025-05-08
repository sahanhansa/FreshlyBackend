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
            // Fetch Item IDs for the Laundry
            var laundryItems = await _context.LaundryItemServices
                .Where(lis => lis.LaundryId == laundryId)
                .Select(lis => lis.ItemId)
                .Distinct()
                .ToListAsync();

            // Return empty list if no items found
            if (!laundryItems.Any())
                return new List<ItemWithServicesDTO>();

            //Fetch Items and Related Data
            var itemsWithServices = await _context.Items
                .Where(i => laundryItems.Contains(i.ItemId))
                .Include(i => i.Category)
                .Include(i => i.LaundryItemServices)
                    .ThenInclude(lis => lis.Service)
                .ToListAsync();

            //Map Data to DTOs
            var result = itemsWithServices.Select(i => new ItemWithServicesDTO
            {
                ItemId = i.ItemId,
                ItemName = i.Name,
                CategoryName = i.Category?.CategoryName ?? "Other", 
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
