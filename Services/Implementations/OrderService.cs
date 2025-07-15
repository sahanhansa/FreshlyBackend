using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
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

        //lasini-confirm new order
        public async Task<bool> ConfirmOrderAsync(ConfirmOrderDTO dto)
        {
            // Begin transaction to ensure data consistency
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Get status IDs
                var orderPlacedStatusId = await _context.Statuses
                    .Where(s => s.StatusName == "Order Placed")
                    .Select(s => s.StatusID)
                    .FirstOrDefaultAsync();

                if (orderPlacedStatusId == Guid.Empty)
                {
                    // Status not found, create it
                    var newStatus = new Status { StatusID = Guid.NewGuid(), StatusName = "Order Placed" };
                    _context.Statuses.Add(newStatus);
                    await _context.SaveChangesAsync();
                    orderPlacedStatusId = newStatus.StatusID;
                }

                // Load the temporary order with customer and laundry details
                var tempOrder = await _context.TemporaryOrders
                    .Include(o => o.Customer)
                    .Include(o => o.Laundry)
                    .FirstOrDefaultAsync(o => o.TemporaryOrderId == dto.TemporaryOrderId);

                if (tempOrder == null)
                    return false;

                // Get all temporary order details
                var tempDetails = await _context.TemporaryOrderDetails
                    .Where(d => d.TemporaryOrderId == dto.TemporaryOrderId)
                    .ToListAsync();

                if (!tempDetails.Any())
                    return false; // No items to confirm

                // Update address if provided
                if (dto.Address != null)
                {
                    var address = await _context.Addresses.FindAsync(dto.Address.AddressId);
                    if (address != null)
                    {
                        address.HouseNo = dto.Address.HouseNo ?? address.HouseNo;
                        address.Street = dto.Address.Street ?? address.Street;
                        address.City = dto.Address.City ?? address.City;
                        address.PostalCode = dto.Address.PostalCode ?? address.PostalCode;
                    }
                }

                // Create new order
                var order = new Order
                {
                    OrderId = Guid.NewGuid(),
                    CustomerId = tempOrder.CustomerId,
                    LaundryId = tempOrder.LaundryId,
                    PlacedAt = DateTime.UtcNow,
                    PickupAt = dto.PickupAt,
                    StatusId = orderPlacedStatusId
                };
                _context.Orders.Add(order);

                // Copy details from temporary order to order details
                foreach (var tempDetail in tempDetails)
                {
                    _context.OrderDetails.Add(new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ItemId = tempDetail.ItemId,
                        ServiceId = tempDetail.ServiceId,
                        Quantity = tempDetail.Quantity
                    });
                }

                // Update temporary order status to "Order Placed"
                tempOrder.StatusId = orderPlacedStatusId;

                // Remove all temporary order details (as specified in requirements)
                _context.TemporaryOrderDetails.RemoveRange(tempDetails);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


    }
}