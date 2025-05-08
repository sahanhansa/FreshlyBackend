using FreshlyBackendNew.Models;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface ITemporyBasketService
    {
        Task AddToBasketAsync(TemporyBasket basketItem);
        Task<List<TemporyBasket>> GetBasketAsync(Guid temporyOrderId);
        Task RemoveFromBasketAsync(TemporyBasket basketItem);
        Task UpdateBasketItemAsync(TemporyBasket basketItem);
    }
}
