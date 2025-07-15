using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
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
                    CategoryName = category != null ? category.CategoryName : "Other",
                    Service = service,
                    Price = laundryItemService.Price
                }).ToListAsync();

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
                            Price = g.Price ?? 0
                        })
                        .ToList()
                })
                .ToList();

            return result;
        }

        public async Task<bool> DeleteItemAsync(Guid id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return false;

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddItemAsync(AddItemDTO itemDto)
        {
            var item = new Item
            {
                ItemId = Guid.NewGuid(),
                Name = itemDto.Name,
                Description = itemDto.Description,
                CategoryId = itemDto.CategoryId,
                ItemImageLink = itemDto.ImageUrl
            };

            _context.Items.Add(item);

            foreach (var service in itemDto.Services)
            {
                var laundryItemService = new LaundryItemService
                {
                    LaundryId = itemDto.LaundryId,
                    ItemId = item.ItemId,
                    ServiceId = service.ServiceId,
                    Price = service.Price ?? 0
                };

                _context.LaundryItemServices.Add(laundryItemService);
            }

            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
