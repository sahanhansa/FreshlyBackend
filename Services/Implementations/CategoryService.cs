using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryIdResponseDTO?> GetCategoryIdByNameAsync(string categoryName)
        {
            var category = await _context.ItemCategories
                .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == categoryName.ToLower());

            if (category == null)
                return null;

            return new CategoryIdResponseDTO
            {
                CategoryId = category.CategoryId
            };
        }
    }
}