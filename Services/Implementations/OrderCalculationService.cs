using FreshlyBackendNew.Data;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Services.Implementations
{
    public class OrderCalculationService : IOrderCalculationService
    {
        private readonly ApplicationDbContext _context;

        public OrderCalculationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> CalculateTotalCostAsync(Guid orderId)
        {
            var orderDetails = await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .ToListAsync();

            decimal total = 0;
            
            foreach (var detail in orderDetails)
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order != null)
                {
                    var price = await _context.LaundryItemServices
                        .Where(lis => lis.LaundryId == order.LaundryId && 
                                      lis.ItemId == detail.ItemId && 
                                      lis.ServiceId == detail.ServiceId)
                        .Select(lis => lis.Price ?? 0)
                        .FirstOrDefaultAsync();
                        
                    total += price * (detail.Quantity ?? 0);
                }
            }
            
            return total;
        }

        public async Task<Dictionary<Guid, decimal>> CalculateTotalCostBulkAsync(IEnumerable<Guid> orderIds)
        {
            var orderIdsList = orderIds.ToList();
            var results = new Dictionary<Guid, decimal>();

            foreach (var orderId in orderIdsList)
            {
                results[orderId] = await CalculateTotalCostAsync(orderId);
            }

            return results;
        }
    }
}
