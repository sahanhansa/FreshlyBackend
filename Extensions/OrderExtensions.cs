using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Extensions
{
    public static class OrderExtensions
    {
        /// <summary>
        /// Calculate total cost for a single order
        /// </summary>
        public static async Task<decimal> CalculateTotalCostAsync(
            this Order order, 
            ApplicationDbContext context)
        {
            var orderDetails = await context.OrderDetails
                .Where(od => od.OrderId == order.OrderId)
                .ToListAsync();

            decimal total = 0;
            
            foreach (var detail in orderDetails)
            {
                var price = await context.LaundryItemServices
                    .Where(lis => lis.LaundryId == order.LaundryId && 
                                  lis.ItemId == detail.ItemId && 
                                  lis.ServiceId == detail.ServiceId)
                    .Select(lis => lis.Price ?? 0)
                    .FirstOrDefaultAsync();
                    
                total += price * (detail.Quantity ?? 0);
            }
            
            return total;
        }

        /// <summary>
        /// Calculate total costs for multiple orders efficiently (bulk operation)
        /// </summary>
        public static async Task<Dictionary<Guid, decimal>> CalculateTotalCostBulkAsync(
            this IEnumerable<Order> orders,
            ApplicationDbContext context)
        {
            var orderIds = orders.Select(o => o.OrderId).ToList();
            
            // Bulk load all order details
            var orderDetailsLookup = await context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId.Value))
                .GroupBy(od => od.OrderId)
                .ToDictionaryAsync(g => g.Key, g => g.ToList());

            // Bulk load all pricing data
            var laundryIds = orders.Select(o => o.LaundryId).Where(id => id.HasValue).Distinct().ToList();
            var pricingLookup = await context.LaundryItemServices
                .Where(lis => laundryIds.Contains(lis.LaundryId))
                .ToDictionaryAsync(
                    lis => (lis.LaundryId, lis.ItemId, lis.ServiceId),
                    lis => lis.Price ?? 0);

            var results = new Dictionary<Guid, decimal>();
            
            foreach (var order in orders)
            {
                decimal total = 0;
                if (orderDetailsLookup.TryGetValue(order.OrderId, out var details))
                {
                    foreach (var detail in details)
                    {
                        var key = (order.LaundryId, detail.ItemId, detail.ServiceId);
                        if (pricingLookup.TryGetValue(key, out var price))
                        {
                            total += price * (detail.Quantity ?? 0);
                        }
                    }
                }
                results[order.OrderId] = total;
            }
            
            return results;
        }
    }
}
