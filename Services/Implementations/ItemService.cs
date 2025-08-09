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
        
        public async Task<ItemWithGarmentTypesDTO?> GetItemByLaundryIdAndItemIdAsync(Guid laundryId, Guid itemId)
        {
            var query = from item in _context.Items
                        join laundryItemService in _context.LaundryItemServices on item.ItemId equals laundryItemService.ItemId
                        join service in _context.Services on laundryItemService.ServiceId equals service.ServiceId
                        join garmentType in _context.GarmentTypes on laundryItemService.GarmentTypeId equals garmentType.GarmentTypeId into gtGroup
                        from garmentType in gtGroup.DefaultIfEmpty()
                        join category in _context.ItemCategories on item.CategoryId equals category.CategoryId into catGroup
                        from category in catGroup.DefaultIfEmpty()
                        where laundryItemService.LaundryId == laundryId && item.ItemId == itemId
                        select new
                        {
                            Item = item,
                            laundryItemService.GarmentTypeId,
                            GarmentTypeName = garmentType != null ? garmentType.GarmentTypeName : null,
                            Service = service,
                            laundryItemService.Price,
                            CategoryName = category != null ? category.CategoryName : string.Empty
                        };

            var results = await query.ToListAsync();
            if (!results.Any())
                return null;

            var first = results.First();
            var garmentTypes = results
                .GroupBy(x => new { x.GarmentTypeId, x.GarmentTypeName })
                .Select(g => new GarmentTypeWithServicesDTO
                {
                    GarmentTypeId = g.Key.GarmentTypeId ?? Guid.Empty,
                    GarmentTypeName = g.Key.GarmentTypeName ?? string.Empty,
                    Services = g.Select(s => new ServiceDTO
                    {
                        ServiceId = s.Service.ServiceId,
                        ServiceName = s.Service.ServiceName,
                        Price = s.Price ?? 0
                    }).ToList()
                }).ToList();

            return new ItemWithGarmentTypesDTO
            {
                ItemId = first.Item.ItemId,
                Name = first.Item.Name,
                Description = first.Item.Description,
                CategoryId = first.Item.CategoryId,
                CategoryName = first.CategoryName,
                ImageUrl = first.Item.ItemImageLink,
                GarmentTypes = garmentTypes
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

            // Add LaundryItemService records for each garment type and its services
            foreach (var garmentType in itemDto.GarmentTypes)
            {
                foreach (var service in garmentType.Services)
                {
                    var laundryItemService = new LaundryItemService
                    {
                        LaundryId = laundryId,
                        ItemId = item.ItemId,
                        GarmentTypeId = garmentType.GarmentTypeId,
                        ServiceId = service.ServiceId,
                        Price = service.Price ?? 0
                    };
                    _context.LaundryItemServices.Add(laundryItemService);
                }
            }

            var result = await _context.SaveChangesAsync();
            return (result > 0, result > 0 ? "Item added successfully." : "Failed to add item.");
        }
        
        //Rohansi-Edit an item
        
        public async Task<bool> UpdateItemAsync(Guid itemId, UpdateItemDTO itemDto, Guid laundryId)
        {
            try
            {
                // Validate input
                if (itemDto == null || itemDto.GarmentTypes == null || !itemDto.GarmentTypes.Any())
                {
                    return false;
                }

                // Check if the item belongs to this laundry
                var isOwned = await _context.LaundryItemServices
                    .AnyAsync(x => x.ItemId == itemId && x.LaundryId == laundryId);

                if (!isOwned)
                    return false;

                // Update the item info
                var item = await _context.Items.FindAsync(itemId);
                if (item == null)
                    return false;

                // Update item properties
                if (!string.IsNullOrEmpty(itemDto.Name))
                    item.Name = itemDto.Name;
                
                if (!string.IsNullOrEmpty(itemDto.Description))
                    item.Description = itemDto.Description;
                
                if (itemDto.CategoryId != Guid.Empty)
                    item.CategoryId = itemDto.CategoryId;
                
                if (!string.IsNullOrEmpty(itemDto.ImageUrl))
                    item.ItemImageLink = itemDto.ImageUrl;

                // Remove existing services for this laundry & item
                var existingServices = await _context.LaundryItemServices
                    .Where(x => x.ItemId == itemId && x.LaundryId == laundryId)
                    .ToListAsync();
                _context.LaundryItemServices.RemoveRange(existingServices);

                // Add updated services grouped by garment type
                foreach (var garmentType in itemDto.GarmentTypes)
                {
                    if (garmentType.Services != null && garmentType.Services.Any())
                    {
                        foreach (var service in garmentType.Services)
                        {
                            if (service.ServiceId != Guid.Empty)
                            {
                                var newService = new LaundryItemService
                                {
                                    LaundryId = laundryId,
                                    ItemId = itemId,
                                    GarmentTypeId = garmentType.GarmentTypeId,
                                    ServiceId = service.ServiceId,
                                    Price = service.Price ?? 0
                                };
                                _context.LaundryItemServices.Add(newService);
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating item: {ex.Message}");
                return false;
            }
        }

        public async Task<(bool success, string message)> AddGarmentTypeAsync(AddGarmentTypeDTO garmentTypeDto)
        {
            try
            {
                // Check if garment type with the same name already exists
                var exists = await _context.GarmentTypes.AnyAsync(g => g.GarmentTypeName.ToLower() == garmentTypeDto.Name.ToLower());
                if (exists)
                {
                    return (false, $"Garment type '{garmentTypeDto.Name}' already exists.");
                }
                var garmentType = new GarmentType
                {
                    GarmentTypeId = Guid.NewGuid(),
                    GarmentTypeName = garmentTypeDto.Name
                };
                _context.GarmentTypes.Add(garmentType);
                await _context.SaveChangesAsync();
                return (true, "Garment type added successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to add garment type: {ex.Message}");
            }
        }
        
        public async Task<Guid?> GetGarmentTypeIdByNameAsync(string name)
        {
            var garmentType = await _context.GarmentTypes
                .FirstOrDefaultAsync(g => g.GarmentTypeName.ToLower() == name.ToLower());
            return garmentType?.GarmentTypeId;
        }
        
    }
}
