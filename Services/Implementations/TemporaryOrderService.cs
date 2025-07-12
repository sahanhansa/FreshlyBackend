using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace FreshlyBackendNew.Services.Implementations
{
    public class TemporaryOrderService(ApplicationDbContext context) : ITemporaryOrderService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Guid> AddToBasketAsync(AddToBasketDTO dto)
        {
            // Check if a temporary order already exists for this customer and laundry
            var tempOrder = await _context.TemporaryOrders
                .FirstOrDefaultAsync(o => o.CustomerId == dto.CustomerId && o.LaundryId == dto.LaundryId);

            if (tempOrder == null)
            {
                tempOrder = new TemporaryOrder
                {
                    TemporaryOrderId = Guid.NewGuid(),
                    CustomerId = dto.CustomerId,
                    LaundryId = dto.LaundryId,
                    PlacedAt = DateTime.UtcNow
                };
                _context.TemporaryOrders.Add(tempOrder);
            }

            foreach (var item in dto.Items)
            {
                // Check if item already exists in basket
                var existingDetail = await _context.TemporaryOrderDetails
                    .FirstOrDefaultAsync(d =>
                        d.TemporaryOrderId == tempOrder.TemporaryOrderId &&
                        d.ItemId == item.ItemId &&
                        d.ServiceId == item.ServiceId);

                if (existingDetail != null)
                {
                    existingDetail.Quantity += item.Quantity;
                }
                else
                {
                    _context.TemporaryOrderDetails.Add(new TemporaryOrderDetail
                    {
                        TemporaryOrderId = tempOrder.TemporaryOrderId,
                        ItemId = item.ItemId,
                        ServiceId = item.ServiceId,
                        Quantity = item.Quantity
                    });
                }
            }

            await _context.SaveChangesAsync();
            return tempOrder.TemporaryOrderId;
        }
    }
}
