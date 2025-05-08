using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        //Lasini- GET request to get items list according to the laundry
        private readonly IItemService _itemService;
        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

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
                return StatusCode(500, new { error = "An error occurred while retrieving items for this laundry", details = ex.Message });
            }
        }
    }
}
