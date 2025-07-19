using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryIdResponseDTO?> GetCategoryIdByNameAsync(string categoryName);
    }
}