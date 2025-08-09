using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemporaryOrderController : ControllerBase
    {
        private readonly ITemporaryOrderService _temporaryOrderService;

        public TemporaryOrderController(ITemporaryOrderService temporaryOrderService)
        {
            _temporaryOrderService = temporaryOrderService;
        }

        //lasini-add to basket
        [HttpPost("add-to-basket")]
        public async Task<IActionResult> AddToBasket([FromBody] AddToBasketDTO dto)
        {
            if (dto == null || dto.Items == null || !dto.Items.Any())
                return BadRequest("Invalid basket data.");

            var tempOrderId = await _temporaryOrderService.AddToBasketAsync(dto);
            return Ok(new { TemporaryOrderId = tempOrderId });
        }

        // lasini- get all temporary order summaries for a customer, grouped by laundry
        [HttpGet("customer/{customerId}/summaries")]
        public async Task<IActionResult> GetCustomerOrderSummaries(Guid customerId)
        {
            var summaries = await _temporaryOrderService.GetCustomerTemporaryOrderSummariesAsync(customerId);
            return Ok(summaries);
        }

        // lasini-delete item from a temporary order (order summary)
        [HttpDelete("{temporaryOrderId}/item/{itemId}/service/{serviceId}/garment/{garmentTypeId}")]
        public async Task<IActionResult> DeleteItemFromOrder(Guid temporaryOrderId, Guid itemId, Guid serviceId, Guid garmentTypeId)
        {
            var result = await _temporaryOrderService.DeleteItemFromTemporaryOrderAsync(temporaryOrderId, itemId, serviceId, garmentTypeId);
            if (!result)
                return NotFound("Item not found in the order.");
            return NoContent();
        }

        // lasini-delete a complete temporary order
        [HttpDelete("{temporaryOrderId}/delete")]
        public async Task<IActionResult> DeleteTemporaryOrder(Guid temporaryOrderId)
        {
            var result = await _temporaryOrderService.DeleteTemporaryOrderAsync(temporaryOrderId);
            if (!result)
                return NotFound("Order not found.");
            return NoContent();
        }
    }
}
