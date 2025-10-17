using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Services.Implementations
{
    public partial class OrderService
    {
        public async Task<List<OrderDTO>> GetNewOrdersAsync(Guid laundryId)
        {
            // Add AsNoTracking for read-only queries - significant performance improvement
            var pickedUpStatus = await _context.Statuses
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StatusName != null && s.StatusName.Trim().ToLower() == "order picked up");

            if (pickedUpStatus == null)
                return new List<OrderDTO>();

            var orders = await _context.Orders
                .AsNoTracking()
                .Where(o => o.LaundryId == laundryId && o.StatusId == pickedUpStatus.StatusID)
                .Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                    PlacedDate = o.PlacedAt.HasValue ? o.PlacedAt.Value.ToString("yyyy-MM-dd") : null,
                    PlacedTime = o.PlacedAt.HasValue ? o.PlacedAt.Value.ToString("HH:mm:ss") : null,
                    PickupDate = o.PickupAt.HasValue ? o.PickupAt.Value.ToString("yyyy-MM-dd") : null,
                    PickupTime = o.PickupAt.HasValue ? o.PickupAt.Value.ToString("HH:mm:ss") : null
                })
                .ToListAsync();

            return orders;
        }
    }
}
