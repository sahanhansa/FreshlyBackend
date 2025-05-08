using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemporyBasketController : ControllerBase
    {
        private readonly ITemporyBasketService _basketService;

        public TemporyBasketController(ITemporyBasketService basketService)
        {
            _basketService = basketService;
        }

        // POST: api/TemporyBasket/AddToBasket
        [HttpPost("AddToBasket")]
        public async Task<IActionResult> AddToBasket([FromBody] TemporyBasketRequestDTO basketRequest)
        {
            try
            {
                if (basketRequest == null || basketRequest.ItemId == null || basketRequest.ServiceId == null)
                    return BadRequest("Invalid basket item data.");

                // Map the DTO to the TemporyBasket entity
                var basketItem = new TemporyBasket
                {
                    TemporyOrderId = basketRequest.TemporyOrderId,
                    ItemId = basketRequest.ItemId,
                    ServiceId = basketRequest.ServiceId,
                    Quantity = basketRequest.Quantity,
                    Price = basketRequest.Price,
                    TotalPrice = basketRequest.Price * basketRequest.Quantity
                };

                await _basketService.AddToBasketAsync(basketItem);
                return Ok("Item added to basket successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddToBasket: {ex.Message}");
                return StatusCode(500, $"An error occurred while adding the item to the basket. {ex.Message}");
            }
        }

        // GET: api/TemporyBasket/GetBasket/{temporyOrderId}
        [HttpGet("GetBasket/{temporyOrderId}")]
        public async Task<IActionResult> GetBasket(Guid temporyOrderId)
        {
            var basketItems = await _basketService.GetBasketAsync(temporyOrderId);

            if (!basketItems.Any())
                return NotFound("No items found in the basket.");

            // Map the TemporyBasket entities to TemporyBasketResponseDTOs
            var response = basketItems.Select(b => new TemporyBasketResponseDTO
            {
                TemporyOrderId = b.TemporyOrderId,
                ItemId = b.ItemId,
                ItemName = b.ItemName,
                ServiceName = b.ServiceName,
                Material = b.Material,
                ImageUrl = b.ImageUrl,
                Quantity = b.Quantity,
                Price = b.Price,
                TotalPrice = b.TotalPrice
            }).ToList();

            return Ok(response);
        }

        // DELETE: api/TemporyBasket/RemoveFromBasket
        [HttpDelete("RemoveFromBasket")]
        public async Task<IActionResult> RemoveFromBasket([FromBody] TemporyBasketRequestDTO basketRequest)
        {
            try
            {
                if (basketRequest == null || basketRequest.ItemId == null || basketRequest.ServiceId == null)
                    return BadRequest("Invalid basket item data.");

                // Map the DTO to the TemporyBasket entity
                var basketItem = new TemporyBasket
                {
                    TemporyOrderId = basketRequest.TemporyOrderId,
                    ItemId = basketRequest.ItemId,
                    ServiceId = basketRequest.ServiceId
                };

                await _basketService.RemoveFromBasketAsync(basketItem);
                return Ok("Item removed from basket successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RemoveFromBasket: {ex.Message}");
                return StatusCode(500, $"An error occurred while removing the item from the basket. {ex.Message}");
            }
        }

        // PUT: api/TemporyBasket/UpdateBasketItem
        [HttpPut("UpdateBasketItem")]
        public async Task<IActionResult> UpdateBasketItem([FromBody] TemporyBasketRequestDTO basketRequest)
        {
            try
            {
                if (basketRequest == null || basketRequest.ItemId == null || basketRequest.ServiceId == null)
                    return BadRequest("Invalid basket item data.");

                // Map the DTO to the TemporyBasket entity
                var basketItem = new TemporyBasket
                {
                    TemporyOrderId = basketRequest.TemporyOrderId,
                    ItemId = basketRequest.ItemId,
                    ServiceId = basketRequest.ServiceId,
                    Quantity = basketRequest.Quantity,
                    Price = basketRequest.Price,
                    TotalPrice = basketRequest.Price * basketRequest.Quantity
                };

                await _basketService.UpdateBasketItemAsync(basketItem);
                return Ok("Basket item updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateBasketItem: {ex.Message}");
                return StatusCode(500, $"An error occurred while updating the basket item. {ex.Message}");
            }
        }
    }
}
