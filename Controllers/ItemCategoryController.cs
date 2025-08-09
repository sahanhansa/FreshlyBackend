using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemCategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public ItemCategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("GetCategoryIdByName/{categoryName}")]
        public async Task<IActionResult> GetCategoryIdByName(string categoryName)
        {
            var result = await _categoryService.GetCategoryIdByNameAsync(categoryName);

            if (result == null)
                return NotFound($"Category with name '{categoryName}' not found.");

            return Ok(result);
        }
    }
}