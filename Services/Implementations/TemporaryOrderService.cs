using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace FreshlyBackendNew.Services.Implementations
{
    public class TemporaryOrderService : ITemporaryOrderService
    {
        private readonly ApplicationDbContext _context;

        public TemporaryOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

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

        public async Task<List<TemporaryOrderSummaryDTO>> GetCustomerTemporaryOrderSummariesAsync(Guid customerId)
        {
            var tempOrders = await _context.TemporaryOrders
                .Include(o => o.Laundry)
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();

            var summaries = new List<TemporaryOrderSummaryDTO>();

            foreach (var tempOrder in tempOrders)
            {
                var details = await _context.TemporaryOrderDetails
                    .Where(d => d.TemporaryOrderId == tempOrder.TemporaryOrderId)
                    .Include(d => d.Item)
                        //.ThenInclude(item => item.Category)
                    .Include(d => d.Service)
                    .ToListAsync();

                var items = new List<TemporaryOrderItemDTO>();
                decimal total = 0;

                foreach (var d in details)
                {
                    var price = await _context.LaundryItemServices
                        .Where(lis => lis.LaundryId == tempOrder.LaundryId && lis.ItemId == d.ItemId && lis.ServiceId == d.ServiceId)
                        .Select(lis => lis.Price ?? 0)
                        .FirstOrDefaultAsync();

                    var itemDto = new TemporaryOrderItemDTO
                    {
                        ItemId = d.ItemId ?? Guid.Empty,
                        ItemName = d.Item?.Name ?? "Item", // Replace with actual property if available
                        ItemImageUrl = null, // Add if you have image URLs
                        //CategoryName = d.Item?.Category?.CategoryName,
                        ServiceId = d.ServiceId,
                        ServiceName = d.Service?.ServiceName ?? "",
                        Price = price,
                        Quantity = d.Quantity ?? 0
                    };
                    total += itemDto.SubTotal;
                    items.Add(itemDto);
                }

                summaries.Add(new TemporaryOrderSummaryDTO
                {
                    TemporaryOrderId = tempOrder.TemporaryOrderId,
                    LaundryId = tempOrder.LaundryId,
                    LaundryName = tempOrder.Laundry?.LaundryName ?? "",
                    LaundryAddress = tempOrder.Laundry != null
                        ? $"{tempOrder.Laundry.LaundryName}, {tempOrder.Laundry.Address}"
                        : "",
                    Items = items,
                    TotalCost = total // Use TotalCost for consistency
                });
            }

            return summaries;
        }

        public async Task<bool> DeleteItemFromTemporaryOrderAsync(Guid temporaryOrderId, Guid itemId, Guid serviceId)
        {
            var detail = await _context.TemporaryOrderDetails
                .FirstOrDefaultAsync(d =>
                    d.TemporaryOrderId == temporaryOrderId &&
                    d.ItemId == itemId &&
                    d.ServiceId == serviceId);

            if (detail == null)
                return false;

            _context.TemporaryOrderDetails.Remove(detail);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTemporaryOrderAsync(Guid temporaryOrderId)
        {
            // Load the order and its details
            var tempOrder = await _context.TemporaryOrders.FirstOrDefaultAsync(o => o.TemporaryOrderId == temporaryOrderId);


            if (tempOrder == null)
                return false;

            // Remove all related details first
            var details = await _context.TemporaryOrderDetails
                .Where(d => d.TemporaryOrderId == temporaryOrderId)
                .ToListAsync();

            _context.TemporaryOrderDetails.RemoveRange(details);
            _context.TemporaryOrders.Remove(tempOrder);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> CalculateTemporaryOrderTotalCostAsync(Guid temporaryOrderId)
        {
            var details = await _context.TemporaryOrderDetails
                .Where(d => d.TemporaryOrderId == temporaryOrderId)
                .ToListAsync();

            var tempOrder = await _context.TemporaryOrders.FindAsync(temporaryOrderId);
            if (tempOrder == null)
                return 0;

            decimal total = 0;
            foreach (var d in details)
            {
                var price = await _context.LaundryItemServices
                    .Where(lis => lis.LaundryId == tempOrder.LaundryId && lis.ItemId == d.ItemId && lis.ServiceId == d.ServiceId)
                    .Select(lis => lis.Price ?? 0)
                    .FirstOrDefaultAsync();
                total += price * (d.Quantity ?? 0);
            }
            return total;
        }
    }

}
