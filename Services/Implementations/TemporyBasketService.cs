using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Services.Implementations
{
    public class TemporyBasketService : ITemporyBasketService
    {
        private readonly ApplicationDbContext _context;

        public TemporyBasketService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddToBasketAsync(TemporyBasket basketItem)
        {
            var existingItem = await _context.TemporyBaskets
                .FirstOrDefaultAsync(tb => tb.TemporyOrderId == basketItem.TemporyOrderId &&
                                           tb.ItemId == basketItem.ItemId &&
                                           tb.ServiceId == basketItem.ServiceId);

            if (existingItem != null)
            {
                existingItem.Quantity += basketItem.Quantity;
                existingItem.TotalPrice = existingItem.Price * existingItem.Quantity;
            }
            else
            {
                basketItem.TotalPrice = basketItem.Price * basketItem.Quantity;
                _context.TemporyBaskets.Add(basketItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<TemporyBasket>> GetBasketAsync(Guid temporyOrderId)
        {
            return await _context.TemporyBaskets
                .Where(tb => tb.TemporyOrderId == temporyOrderId)
                .ToListAsync();
        }

        public async Task RemoveFromBasketAsync(TemporyBasket basketItem)
        {
            var existingItem = await _context.TemporyBaskets
                .FirstOrDefaultAsync(tb => tb.TemporyOrderId == basketItem.TemporyOrderId &&
                                           tb.ItemId == basketItem.ItemId &&
                                           tb.ServiceId == basketItem.ServiceId);

            if (existingItem != null)
            {
                _context.TemporyBaskets.Remove(existingItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateBasketItemAsync(TemporyBasket basketItem)
        {
            var existingItem = await _context.TemporyBaskets
                .FirstOrDefaultAsync(tb => tb.TemporyOrderId == basketItem.TemporyOrderId &&
                                           tb.ItemId == basketItem.ItemId &&
                                           tb.ServiceId == basketItem.ServiceId);

            if (existingItem != null)
            {
                existingItem.Quantity = basketItem.Quantity;
                existingItem.TotalPrice = existingItem.Price * basketItem.Quantity;
                await _context.SaveChangesAsync();
            }
        }
    }
}
