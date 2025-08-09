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
        
        [HttpPost("add-item/{laundryId}")]
        public async Task<IActionResult> AddItem([FromBody] AddItemDTO itemDto, Guid laundryId)
        {
            if (itemDto == null || itemDto.GarmentTypes == null || !itemDto.GarmentTypes.Any() || itemDto.GarmentTypes.Any(g => g.Services == null || !g.Services.Any()))
            {
                return BadRequest("Item details or garment type services are missing.");
            }

            // Use the provided laundryId parameter from the route
            var (success, message) = await _itemService.AddItemAsync(itemDto, laundryId);

            return success ? Ok(message) : StatusCode(500, message);
        }

        
        //Rohansi-Edit an item from list
        [HttpPut("update-item/{itemId}/{laundryId}")]
        public async Task<IActionResult> UpdateItem(Guid itemId, Guid laundryId, [FromBody] UpdateItemDTO itemDto)
        {
            try
            {
                // Validate input
                if (itemDto == null)
                {
                    return BadRequest("Item data is required.");
                }

                if (itemDto.GarmentTypes == null || !itemDto.GarmentTypes.Any())
                {
                    return BadRequest("At least one garment type with services is required.");
                }

                // Validate that each garment type has services
                foreach (var garmentType in itemDto.GarmentTypes)
                {
                    if (garmentType.Services == null || !garmentType.Services.Any())
                    {
                        return BadRequest($"Garment type {garmentType.GarmentTypeId} must have at least one service.");
                    }
                }

                var result = await _itemService.UpdateItemAsync(itemId, itemDto, laundryId);
                if (!result)
                    return Unauthorized("You cannot update this item or item not found.");

                return Ok("Item updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateItem: {ex.Message}");
                return StatusCode(500, $"An error occurred while updating the item: {ex.Message}");
            }
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

        [HttpGet("GetItemByLaundryId/{laundryId}/{itemId}")]
        public async Task<IActionResult> GetItemByLaundryIdAndItemId(Guid laundryId, Guid itemId)
        {
            var result = await _itemService.GetItemByLaundryIdAndItemIdAsync(laundryId, itemId);
            if (result == null)
                return NotFound($"Item with ID {itemId} not found for laundry {laundryId}.");
            return Ok(result);
        }

        // Add new garment type
        [HttpPost("add-garment-type")]
        public async Task<IActionResult> AddGarmentType([FromBody] AddGarmentTypeDTO garmentTypeDto)
        {
            if (garmentTypeDto == null || string.IsNullOrWhiteSpace(garmentTypeDto.Name))
            {
                return BadRequest("Garment type name is required.");
            }
            var (success, message) = await _itemService.AddGarmentTypeAsync(garmentTypeDto);
            return success ? Ok(message) : StatusCode(500, message);
        }

        // GET: api/Item/garment-type-id-by-name/{name}
        [HttpGet("garment-type-id-by-name/{name}")]
        public async Task<IActionResult> GetGarmentTypeIdByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Garment type name is required.");
            
            var garmentTypeId = await _itemService.GetGarmentTypeIdByNameAsync(name);
            if (garmentTypeId == null)
            {
                // Automatically create the garment type if it doesn't exist
                var addGarmentTypeDto = new AddGarmentTypeDTO { Name = name };
                var (success, message) = await _itemService.AddGarmentTypeAsync(addGarmentTypeDto);
                
                if (success)
                {
                    // Get the newly created garment type ID
                    garmentTypeId = await _itemService.GetGarmentTypeIdByNameAsync(name);
                    return Ok(new { garmentTypeId, message = "Garment type created successfully" });
                }
                else
                {
                    return Ok(new { garmentTypeId = false, message });
                }
            }
            
            return Ok(new { garmentTypeId });
        }

        [HttpGet("GetItemsByLaundryId/{laundryId}/{garmentTypeId}")]
        public async Task<IActionResult> GetItemsByLaundryId(Guid laundryId, Guid garmentTypeId)
        {
            var result = await _itemService.GetItemsByLaundryIdAsync(laundryId, garmentTypeId);
            return Ok(result);
        }
    }

    }
