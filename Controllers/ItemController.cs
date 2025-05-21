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
        //Rohansi-Edit item details
        //Rohansi-Delete an item from list
            
         [HttpDelete("delete-item/{itemId}")]
            public async Task<IActionResult> DeleteItem(Guid itemId)
            {
                try
                {
                    var result = await _itemService.DeleteItemAsync(itemId);
                    if (!result)
                        return NotFound($"Item with ID {itemId} not found.");

                    return NoContent(); // 204
                }
                catch (Exception ex)
                {
                    // Log the error if needed
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }

    }
