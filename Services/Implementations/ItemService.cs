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
            // Fetch Items and Related Data using explicit joins
            var itemsWithServices = await (from item in _context.Items
                                           join laundryItemService in _context.LaundryItemServices
                                           on item.ItemId equals laundryItemService.ItemId
                                           join service in _context.Services
                                           on laundryItemService.ServiceId equals service.ServiceId into serviceGroup
                                           from service in serviceGroup.DefaultIfEmpty()
                                           join category in _context.ItemCategories
                                           on item.CategoryId equals category.CategoryId into categoryGroup
                                           from category in categoryGroup.DefaultIfEmpty()
                                           where laundryItemService.LaundryId == laundryId
                                           select new
                                           {
                                               Item = item,
                                               CategoryName = category != null ? category.CategoryName : "Other", // Replace null-propagating operator
                                               Service = service,
                                               Price = laundryItemService.Price
                                           }).ToListAsync();

            // Map Data to DTOs
            var result = itemsWithServices
                .GroupBy(i => i.Item.ItemId)
                .Select(group => new ItemWithServicesDTO
                {
                    ItemId = group.Key,
                    ItemName = group.First().Item.Name,
                    CategoryName = group.First().CategoryName,
                    Services = group
                        .Where(g => g.Service != null)
                        .Select(g => new ServiceWithPriceDTO
                        {
                            ServiceId = g.Service.ServiceId,
                            ServiceName = g.Service.ServiceName,
                            Price = g.Price ?? 0 // Handle null Price
                        })
                        .ToList()
                })
                .ToList();

            return result;
        }


    }
}
