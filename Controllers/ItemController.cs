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

        //Rohansi-Add new item to a list
        
        [HttpPost("add-item/{laundryId}")]
        public async Task<IActionResult> AddItem(Guid laundryId, [FromBody] AddItemDTO itemDto)
        {
            if (itemDto == null || itemDto.Services == null || !itemDto.Services.Any())
            {
                return BadRequest("Item details or services are missing.");
            }

            var success = await _itemService.AddItemAsync(itemDto, laundryId);

            return success ? Ok("Item added successfully.") : StatusCode(500, "Failed to add item.");
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

        
        //Rohansi-Delete an item from list
            
        [HttpDelete("delete-item/{itemId}/{laundryId}")]
        public async Task<IActionResult> DeleteItem(Guid itemId, Guid laundryId)
        {
            var result = await _itemService.DeleteItemAsync(itemId, laundryId);
            if (!result)
                return NotFound($"Item with ID {itemId} not found or not owned by your laundry.");

            return NoContent();
        }
    }

    }
