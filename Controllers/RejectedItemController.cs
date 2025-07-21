using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RejectedItemController : ControllerBase
    {
        private readonly IRejectedItemService _rejectedItemService;

        public RejectedItemController(IRejectedItemService rejectedItemService)
        {
            _rejectedItemService = rejectedItemService;
        }

        // GET: api/RejectedItem/{rejectedItemId}
        [HttpGet("{rejectedItemId:guid}")]
        public async Task<IActionResult> GetRejectedItemById(Guid rejectedItemId)
        {
            var dto = await _rejectedItemService.GetRejectedItemByIdAsync(rejectedItemId);
            if (dto == null)
                return NotFound($"RejectedItem with RejectedItemId '{rejectedItemId}' not found.");

            return Ok(dto);
        }

        // POST: api/RejectedItem
        [HttpPost]
        public async Task<IActionResult> CreateRejectedItem([FromBody] RejectedItemDTO rejectedItemDto)
        {
            if (rejectedItemDto == null)
                return BadRequest("RejectedItem data is null.");

            var createdDto = await _rejectedItemService.AddRejectedItemAsync(rejectedItemDto);

            return CreatedAtAction(nameof(GetRejectedItemById), new { rejectedItemId = createdDto.RejectedItemId }, createdDto);
        }
    }
}