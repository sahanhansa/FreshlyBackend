using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        //Rohansi-Get new Orders
        public async Task<List<OrderDTO>> GetNewOrdersAsync(Guid laundryId)
        {
            var pickedUpStatus = await _context.Statuses
                .FirstOrDefaultAsync(s => s.StatusName.ToLower() == "picked up");

            if (pickedUpStatus == null)
                return new List<OrderDTO>();

            return await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.StatusId == pickedUpStatus.StatusID)
                .Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                    PlacedDate = o.PlacedAt
                })
                .ToListAsync();
        }

        // Rohansi-Get processing orders
        public async Task<List<OrderDTO>> GetProcessingOrdersAsync(Guid laundryId)
        {
            var processingStatus = await _context.Statuses
                .FirstOrDefaultAsync(s => s.StatusName.ToLower() == "processing in laundry");

            if (processingStatus == null)
                return new List<OrderDTO>();

            return await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.StatusId == processingStatus.StatusID)
                .Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                    PlacedDate = o.PlacedAt
                })
                .ToListAsync();
        }

        //Rohansi-Get all order details
        public async Task<List<OrderDTO>> GetAllOrdersAsync(Guid laundryId)
        {
            var excludedStatuses = new List<string> { "order places", "pickup pending", "picked up" };

            return await _context.Orders
                .Where(o => o.LaundryId == laundryId &&
                            !excludedStatuses.Contains(o.Status!.StatusName.ToLower()))
                .Include(o => o.Status)
                .Include(o => o.Customer)
                .Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                    CustomerFName = o.Customer!.FirstName,
                    CustomerLName = o.Customer.LastName,
                    PlacedDate = o.PlacedAt,
                    StatusName = o.Status.StatusName
                })
                .ToListAsync();
        }
    }
}