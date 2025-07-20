using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly IFileStorageService _fileStorageService;

        public ItemController(IItemService itemService, IFileStorageService fileStorageService)
        {
            _itemService = itemService;
            _fileStorageService = fileStorageService;
        }

        //Lasini- GET request to get items list according to the laundry
        [HttpGet("GetItemsByLaundryId/{laundryId}")]
        public async Task<IActionResult> GetItemsByLaundryId(Guid laundryId)
        {
            try
            {
                // Fetch the list of items and their services for the specified laundry
                var result = await _itemService.GetItemsByLaundryIdAsync(laundryId);
                
                // Enhance image URLs if they are filenames
                foreach (var item in result)
                {
                    if (!string.IsNullOrEmpty(item.ImageUrl) && !item.ImageUrl.StartsWith("http"))
                    {
                        try
                        {
                            // Try to get the full image URL from the storage service
                            var fullImageUrl = await _fileStorageService.GetImageUrlAsync(item.ImageUrl);
                            if (!string.IsNullOrEmpty(fullImageUrl))
                            {
                                item.ImageUrl = fullImageUrl;
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log the error but don't fail the entire request
                            Console.WriteLine($"Error resolving image URL for item {item.ItemId}: {ex.Message}");
                        }
                    }
                }
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                Console.WriteLine($"Error in GetItemsByLaundryId: {ex.Message}");
                return StatusCode(500,
                    new { error = "An error occurred while retrieving items for this laundry", details = ex.Message });
            }
        }

        // GET request to get a single item by laundry ID and item ID
        [HttpGet("GetItemByLaundryId/{laundryId}/{itemId}")]
        public async Task<IActionResult> GetItemByLaundryId(Guid laundryId, Guid itemId)
        {
            try
            {
                // Fetch the specific item for the specified laundry
                var result = await _itemService.GetItemByLaundryIdAsync(laundryId, itemId);
                
                if (result == null)
                {
                    return NotFound($"Item with ID {itemId} not found for laundry {laundryId}");
                }
                
                // Enhance image URL if it is a filename
                if (!string.IsNullOrEmpty(result.ImageUrl) && !result.ImageUrl.StartsWith("http"))
                {
                    try
                    {
                        // Try to get the full image URL from the storage service
                        var fullImageUrl = await _fileStorageService.GetImageUrlAsync(result.ImageUrl);
                        if (!string.IsNullOrEmpty(fullImageUrl))
                        {
                            result.ImageUrl = fullImageUrl;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error but don't fail the entire request
                        Console.WriteLine($"Error resolving image URL for item {result.ItemId}: {ex.Message}");
                    }
                }
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                Console.WriteLine($"Error in GetItemByLaundryId: {ex.Message}");
                return StatusCode(500,
                    new { error = "An error occurred while retrieving the item", details = ex.Message });
            }
        }

        //Rohansi-Add new item to a list
        
        [HttpPost("add-item/{laundryId}")]
        public async Task<IActionResult> AddItem(Guid laundryId, [FromBody] AddItemDTO itemDto)
        {
            if (itemDto == null || itemDto.Services == null || !itemDto.Services.Any())
            {
                return BadRequest("Item details or services are missing.");
            }

            var (success, message) = await _itemService.AddItemAsync(itemDto, laundryId);

            if (success)
            {
                return Ok(new { message = message });
            }
            else
            {
                // Return BadRequest for all errors including duplicates
                return BadRequest(new { message = message });
            }
        }

        
        //Rohansi-Edit an item from list
        [HttpPut("update-item/{itemId}/{laundryId}")]
        public async Task<IActionResult> UpdateItem(Guid itemId, Guid laundryId, [FromBody] UpdateItemDTO itemDto)
        {
            var result = await _itemService.UpdateItemAsync(itemId, itemDto, laundryId);
            if (!result)
                return Unauthorized("You cannot update this item or item not found.");

            return Ok("Item updated successfully.");
        }
        
        // Rohansi - Partially update an item (alternative method)
        [HttpPatch("update-item/{itemId}/{laundryId}")]
        public async Task<IActionResult> PatchItem(Guid itemId, Guid laundryId, [FromBody] UpdateItemDTO itemDto)
        {
            var result = await _itemService.UpdateItemAsync(itemId, itemDto, laundryId);
            if (!result)
                return Unauthorized("You cannot update this item or item not found.");

            return Ok("Item updated successfully.");
        }


        
        //Rohansi-Delete an item from list
            
        [HttpDelete("delete-item/{itemId}/{laundryId}")]
        public async Task<IActionResult> DeleteItem(Guid itemId, Guid laundryId)
        {
            try
            {
                // Get the item's image URL before deletion
                var imageUrl = await _itemService.GetItemImageUrlAsync(itemId, laundryId);
                
                if (imageUrl == null)
                {
                    return NotFound($"Item with ID {itemId} not found or not owned by your laundry.");
                }

                // Delete the item from database
                var result = await _itemService.DeleteItemAsync(itemId, laundryId);
                if (!result)
                {
                    return NotFound($"Item with ID {itemId} not found or not owned by your laundry.");
                }

                // Delete the image from storage if it exists
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    try
                    {
                        // Extract filename from URL if it's a full URL
                        string fileName = imageUrl;
                        if (imageUrl.StartsWith("http"))
                        {
                            // Extract filename from URL
                            fileName = Path.GetFileName(new Uri(imageUrl).LocalPath);
                        }

                        // Delete the image from storage
                        var imageDeleted = await _fileStorageService.DeleteImageAsync(fileName);
                        if (!imageDeleted)
                        {
                            // Log the warning but don't fail the request
                            Console.WriteLine($"Warning: Failed to delete image {fileName} for item {itemId}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error but don't fail the request
                        Console.WriteLine($"Error deleting image for item {itemId}: {ex.Message}");
                    }
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteItem: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the item.");
            }
        }
    }

    }
