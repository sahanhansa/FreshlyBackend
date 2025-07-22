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
                    Price = laundryItemService.Price,
                    GarmentType = laundryItemService.GarmentType,
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
                            Price = g.Price ?? 0,
                            GarmentTypeId = g.GarmentType?.GarmentTypeId,
                            GarmentTypeName = g.GarmentType?.GarmentTypeName
                        })
                        .ToList(),
                })
                .ToList();

            return result;
        }
        
        public async Task<ItemWithServicesDTO> GetItemByLaundryIdAsync(Guid laundryId, Guid itemId)
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
                where laundryItemService.LaundryId == laundryId && item.ItemId == itemId
                select new
                {
                    Item = item,
                    CategoryName = category != null ? category.CategoryName : "Other",
                    Service = service,
                    Price = laundryItemService.Price
                }).ToListAsync();

            if (!itemsWithServices.Any())
            {
                return null;
            }

            var result = new ItemWithServicesDTO
            {
                ItemId = itemsWithServices.First().Item.ItemId,
                ItemName = itemsWithServices.First().Item.Name,
                CategoryName = itemsWithServices.First().CategoryName,
                ImageUrl = itemsWithServices.First().Item.ItemImageLink,
                Services = itemsWithServices
                    .Where(g => g.Service != null)
                    .Select(g => new ServiceWithPriceDTO
                    {
                        ServiceId = g.Service.ServiceId,
                        ServiceName = g.Service.ServiceName,
                        Price = g.Price ?? 0
                    })
                    .ToList()
            };

            return result;
        }
        
        public async Task<ItemWithServicesDTO?> GetItemByLaundryIdAndItemIdAsync(Guid laundryId, Guid itemId)
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
                where laundryItemService.LaundryId == laundryId && item.ItemId == itemId
                select new
                {
                    Item = item,
                    CategoryName = category != null ? category.CategoryName : "Other",
                    Service = service,
                    Price = laundryItemService.Price
                }).ToListAsync();

            if (!itemsWithServices.Any())
                return null;

            var group = itemsWithServices.GroupBy(i => i.Item.ItemId).First();
            return new ItemWithServicesDTO
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
            };
        }

        public async Task<List<ItemWithServicesDTO>> GetItemsByLaundryIdAsync(Guid laundryId, Guid garmentTypeId)
        {
            var itemsWithServices = await (from item in _context.Items
                                           join laundryItemService in _context.LaundryItemServices
                                               on item.ItemId equals laundryItemService.ItemId
                                           join service in _context.Services
                                               on laundryItemService.ServiceId equals service.ServiceId
                                           join category in _context.ItemCategories
                                               on item.CategoryId equals category.CategoryId
                                           join garmentType in _context.GarmentTypes
                                               on laundryItemService.GarmentTypeId equals garmentType.GarmentTypeId
                                           where laundryItemService.LaundryId == laundryId
                                               && laundryItemService.GarmentTypeId == garmentTypeId
                                           select new
                                           {
                                               Item = item,
                                               CategoryName = category.CategoryName,
                                               Service = service,
                                               Price = laundryItemService.Price,
                                               GarmentType = garmentType
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
                        .Select(g => new ServiceWithPriceDTO
                        {
                            ServiceId = g.Service.ServiceId,
                            ServiceName = g.Service.ServiceName,
                            Price = g.Price ?? 0,
                            GarmentTypeId = g.GarmentType.GarmentTypeId,
                            GarmentTypeName = g.GarmentType.GarmentTypeName
                        })
                        .ToList(),
                    GarmentTypes = group
                        .Select(g => new GarmentTypeDTO
                        {
                            GarmentTypeId = g.GarmentType.GarmentTypeId,
                            GarmentTypeName = g.GarmentType.GarmentTypeName
                        })
                        .Distinct()
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

            // Get the item to retrieve its image URL before deleting
            var item = await _context.Items.FindAsync(itemId);
            if (item == null)
                return false;

            // Store the image URL for deletion
            var imageUrl = item.ItemImageLink;

            // Delete LaundryItemService entries first
            var relatedServices = _context.LaundryItemServices
                .Where(l => l.ItemId == itemId && l.LaundryId == laundryId);
            _context.LaundryItemServices.RemoveRange(relatedServices);

            // Delete the item itself
            _context.Items.Remove(item);

            await _context.SaveChangesAsync();

            // Return the image URL so the controller can delete it from storage
            return true;
        }

        // Helper method to get item image URL for deletion
        public async Task<string> GetItemImageUrlAsync(Guid itemId, Guid laundryId)
        {
            // Check if the item is associated with the given laundry
            var exists = await _context.LaundryItemServices
                .AnyAsync(l => l.ItemId == itemId && l.LaundryId == laundryId);

            if (!exists)
                return null; // Not allowed to access

            var item = await _context.Items.FindAsync(itemId);
            return item?.ItemImageLink;
        }


        //Rohansi-Add an item
        public async Task<(bool success, string message)> AddItemAsync(AddItemDTO itemDto, Guid laundryId)
        {
            // Check if item with same name and category already exists in this laundry
            var existingItem = await (from existingItemQuery in _context.Items
                join laundryItemService in _context.LaundryItemServices
                    on existingItemQuery.ItemId equals laundryItemService.ItemId
                where laundryItemService.LaundryId == laundryId 
                      && existingItemQuery.Name.ToLower().Trim() == itemDto.Name.ToLower().Trim()
                      && existingItemQuery.CategoryId == itemDto.CategoryId
                select existingItemQuery).FirstOrDefaultAsync();

            if (existingItem != null)
            {
                return (false, $"Item '{itemDto.Name}' already exists in this laundry with the same category.");
            }

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
            return (result > 0, result > 0 ? "Item added successfully." : "Failed to add item.");
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
