using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs.Order_DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Services.Implementations
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailService(ApplicationDbContext context)
        {
            _context = context;
        }

        //lasini-get ongoing order details
        public async Task<OrderDetailsDTO> GetOrderDetailsAsync(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Laundry)
                    .ThenInclude(l => l.Address)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return null;

            // Get order details
            var orderDetails = await _context.OrderDetails
                .Include(od => od.Item)
                .Include(od => od.Service)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();

            var dto = new OrderDetailsDTO
            {
                OrderId = order.OrderId,
                OrderIdFormatted = $"#{order.OrderId.ToString().Substring(0, 6)}",
                OrderDate = order.PlacedAt,
                OrderDateFormatted = order.PlacedAt?.ToString("dd MMMM yyyy"),
                LaundryName = order.Laundry?.LaundryName,
                LaundryLocation = order.Laundry?.Address != null
                    ? $"{order.Laundry.Address.City}"
                    : null,
                Status = order.Status?.StatusName,
                PickupDate = order.PickupAt,
                PickupDateFormatted = order.PickupAt?.ToString("dd MMMM yyyy HH:mm"),
                // Show pickup details only for orders in "Order Placed" status and not yet picked up
                ShouldShowPickupDetails = order.Status?.StatusName?.ToLower() == "order placed" &&
                                          order.PickupAt.HasValue &&
                                          order.PickupAt > DateTime.UtcNow
            };

            // Calculate total
            dto.TotalAmount = await CalculateOrderTotalAsync(orderId);

            // Add order items
            foreach (var detail in orderDetails)
            {
                dto.Items.Add(new OrderItemDTO
                {
                    ItemId = detail.ItemId.Value,
                    ItemName = detail.Item?.Name,
                    ServiceId = detail.ServiceId.Value,
                    ServiceName = detail.Service?.ServiceName,
                    Quantity = detail.Quantity ?? 0,
                    // Here you would calculate the price from your business logic
                    Price = await GetItemServicePriceAsync(detail.ItemId.Value, detail.ServiceId.Value, order.LaundryId.Value)
                });
            }

            return dto;
        }


        //lasini
        public async Task<List<OrderDetailsDTO>> GetOngoingOrdersForCustomerAsync(Guid customerId)
        {
            // Define statuses for "ongoing" orders - modify these based on your business logic
            var ongoingStatuses = new[] { "order placed", "order picked up", "processing in laundry", "finished processing", "out for delivery" };

            var ongoingStatusIds = await _context.Statuses
                .Where(s => ongoingStatuses.Contains(s.StatusName.ToLower()))
                .Select(s => s.StatusID)
                .ToListAsync();

            var orderIds = await _context.Orders
                .Where(o => o.CustomerId == customerId && ongoingStatusIds.Contains(o.StatusId.Value))
                .Select(o => o.OrderId)
                .ToListAsync();

            var result = new List<OrderDetailsDTO>();

            foreach (var orderId in orderIds)
            {
                var orderDetails = await GetOrderDetailsAsync(orderId);
                if (orderDetails != null)
                {
                    result.Add(orderDetails);
                }
            }

            // Order by most recent first
            return result.OrderByDescending(o => o.OrderDate).ToList();
        }

        public async Task<decimal> CalculateOrderTotalAsync(Guid orderId)
        {
            decimal total = 0;

            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || !order.LaundryId.HasValue)
                return 0;

            var details = await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .ToListAsync();

            foreach (var detail in details)
            {
                if (!detail.ItemId.HasValue || !detail.ServiceId.HasValue || !detail.Quantity.HasValue)
                    continue;

                var price = await GetItemServicePriceAsync(
                    detail.ItemId.Value,
                    detail.ServiceId.Value,
                    order.LaundryId.Value);

                total += price * detail.Quantity.Value;
            }

            return total;
        }

        private async Task<decimal> GetItemServicePriceAsync(Guid itemId, Guid serviceId, Guid laundryId)
        {
            var laundryItemService = await _context.LaundryItemServices
                .FirstOrDefaultAsync(lis =>
                    lis.ItemId == itemId &&
                    lis.ServiceId == serviceId &&
                    lis.LaundryId == laundryId);

            return laundryItemService?.Price ?? 0;
        }

       
        // Completely deletes an order and all related records from the database
        public async Task<bool> CancelOrderAsync(Guid orderId)
        {
            // Use a transaction to ensure all related records are deleted or none
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Check if order exists
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                    return false;

                // Delete dependent records in correct order to maintain referential integrity

                // 1. Delete any feedback related to this order
                var feedback = await _context.Feedbacks
                    .Where(f => f.OrderId == orderId)
                    .ToListAsync();
                if (feedback.Any())
                {
                    _context.Feedbacks.RemoveRange(feedback);
                }

                // 2. Delete any driver notes related to this order
                var driverNotes = await _context.DriverNotes
                    .Where(dn => dn.OrderId == orderId)
                    .ToListAsync();
                if (driverNotes.Any())
                {
                    _context.DriverNotes.RemoveRange(driverNotes);
                }

                // 3. Delete order details (items in the order)
                var orderDetails = await _context.OrderDetails
                    .Where(od => od.OrderId == orderId)
                    .ToListAsync();
                if (orderDetails.Any())
                {
                    _context.OrderDetails.RemoveRange(orderDetails);
                }

                // 4. Finally delete the order itself
                _context.Orders.Remove(order);

                // Save all changes
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                // Rollback transaction in case of any errors
                await transaction.RollbackAsync();
                throw; // Rethrow to be handled by controller
            }
        }
    }
}