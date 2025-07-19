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

        public async Task<bool> CheckItemBelongsToLaundry(Guid itemId, Guid laundryId)
        {
            return await _context.LaundryItemServices
                .AnyAsync(x => x.ItemId == itemId && x.LaundryId == laundryId);
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
                    ImageUrl = group.First().Item.ItemImageLink,
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

        
        //Rohansi-Delete an item
        public async Task<bool> DeleteItemAsync(Guid itemId, Guid laundryId)
        {
            // Check if the item is associated with the given laundry
            var exists = await _context.LaundryItemServices
                .AnyAsync(l => l.ItemId == itemId && l.LaundryId == laundryId);

            if (!exists)
                return false; // Not allowed to delete

            // Delete LaundryItemService entries first
            var relatedServices = _context.LaundryItemServices
                .Where(l => l.ItemId == itemId && l.LaundryId == laundryId);
            _context.LaundryItemServices.RemoveRange(relatedServices);

            // Delete the item itself
            var item = await _context.Items.FindAsync(itemId);
            if (item == null)
                return false;

            _context.Items.Remove(item);

            await _context.SaveChangesAsync();
            return true;
        }


        //Rohansi-Add an item
        public async Task<bool> AddItemAsync(AddItemDTO itemDto, Guid laundryId)
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
                    LaundryId = laundryId,
                    ItemId = item.ItemId,
                    ServiceId = service.ServiceId,
                    Price = service.Price ?? 0
                };

                _context.LaundryItemServices.Add(laundryItemService);
            }

            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        
        //Rohansi-Edit an item
        
        public async Task<bool> UpdateItemAsync(Guid itemId, UpdateItemDTO itemDto, Guid laundryId)
        {
            // Check if the item belongs to this laundry
            var isOwned = await _context.LaundryItemServices
                .AnyAsync(x => x.ItemId == itemId && x.LaundryId == laundryId);

            if (!isOwned)
                return false;

            // Update the item info
            var item = await _context.Items.FindAsync(itemId);
            if (item == null)
                return false;

            item.Name = itemDto.Name;
            item.Description = itemDto.Description;
            item.CategoryId = itemDto.CategoryId;
            item.ItemImageLink = itemDto.ImageUrl;

            // Remove existing services for this laundry & item
            var existingServices = _context.LaundryItemServices
                .Where(x => x.ItemId == itemId && x.LaundryId == laundryId);
            _context.LaundryItemServices.RemoveRange(existingServices);

            // Add updated services
            foreach (var service in itemDto.Services)
            {
                var newService = new LaundryItemService
                {
                    LaundryId = laundryId,
                    ItemId = itemId,
                    ServiceId = service.ServiceId,
                    Price = service.Price ?? 0
                };
                _context.LaundryItemServices.Add(newService);
            }

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
