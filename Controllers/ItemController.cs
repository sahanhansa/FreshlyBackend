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

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }
        
        private Guid GetLaundryIdFromToken()
        {
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            return Guid.Parse(userIdClaim);
        }


        //Lasini- GET request to get items list according to the laundry
        [HttpGet("GetItemsByLaundryId/{laundryId}")]
        public async Task<IActionResult> GetItemsByLaundryId(Guid laundryId)
        {
            try
            {
                // Fetch the list of items and their services for the specified laundry
                var result = await _itemService.GetItemsByLaundryIdAsync(laundryId);
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
        
        [HttpPost("add-item")]
        public async Task<IActionResult> AddItem([FromBody] AddItemDTO itemDto)
        {
            if (itemDto == null || itemDto.Services == null || !itemDto.Services.Any())
            {
                return BadRequest("Item details or services are missing.");
            }

            var laundryId = GetLaundryIdFromToken();
            var (success, message) = await _itemService.AddItemAsync(itemDto, laundryId);

            return success ? Ok(message) : StatusCode(500, message);

        }

        
        //Rohansi-Edit an item from list
        [HttpPut("update-item/{itemId}")]
        public async Task<IActionResult> UpdateItem(Guid itemId, [FromBody] UpdateItemDTO itemDto)
        {
            var laundryId = GetLaundryIdFromToken();

            var result = await _itemService.UpdateItemAsync(itemId, itemDto, laundryId);
            if (!result)
                return Unauthorized("You cannot update this item or item not found.");

            return Ok("Item updated successfully.");
        }

        
        //Rohansi-Delete an item from list
            
        [HttpDelete("delete-item/{itemId}")]
        public async Task<IActionResult> DeleteItem(Guid itemId)
        {
            var laundryId = GetLaundryIdFromToken();

            var result = await _itemService.DeleteItemAsync(itemId, laundryId);
            if (!result)
                return NotFound($"Item with ID {itemId} not found or not owned by your laundry.");

            return NoContent();
        }
    }

    }
