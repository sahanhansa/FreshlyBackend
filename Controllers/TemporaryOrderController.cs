using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
}
