using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using FreshlyBackendNew.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Services.Implementations
{
    public class OrderService(ApplicationDbContext context) : IOrderService
    {
        private readonly ApplicationDbContext _context = context;
        
        public async Task<List<OrderDTO>> GetNewOrdersAsync(Guid laundryId)
        {
            var pickedUpStatus = await _context.Statuses
                .FirstOrDefaultAsync(s => s.StatusName != null && string.Equals(s.StatusName, "picked up", StringComparison.OrdinalIgnoreCase));

            if (pickedUpStatus == null)
                return [];

            var orders = await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.StatusId == pickedUpStatus.StatusID)
                .ToListAsync();

            var result = new List<OrderDTO>();
            foreach (var o in orders)
            {
                result.Add(new OrderDTO
                {
                    OrderId = o.OrderId,
                    PlacedDate = o.PlacedAt.HasValue ? o.PlacedAt.Value.ToString("yyyy-MM-dd") : null,
                    PlacedTime = o.PlacedAt.HasValue ? o.PlacedAt.Value.ToString("HH:mm:ss") : null,
                    PickupDate = o.PickupAt.HasValue ? o.PickupAt.Value.ToString("yyyy-MM-dd") : null,
                    PickupTime = o.PickupAt.HasValue ? o.PickupAt.Value.ToString("HH:mm:ss") : null
                });
            }

            return result;
        }

        public async Task<List<OrderDTO>> GetProcessingOrdersAsync(Guid laundryId)
        {
            var processingStatus = await _context.Statuses
                .FirstOrDefaultAsync(s => s.StatusName != null && string.Equals(s.StatusName, "processing in laundry", StringComparison.OrdinalIgnoreCase));

            if (processingStatus == null)
                return [];

            var orders = await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.StatusId == processingStatus.StatusID)
                .Include(o => o.Customer)
                .ThenInclude(c => c.Address)
                .Include(o => o.Laundry)
                .Include(o => o.Status)
                .ToListAsync();

            var result = new List<OrderDTO>();
            foreach (var o in orders)
            {
                var dto = new OrderDTO
                {
                    OrderId = o.OrderId,
                    PlacedDate = o.PlacedAt.HasValue ? o.PlacedAt.Value.ToString("yyyy-MM-dd") : null,
                    PlacedTime = o.PlacedAt.HasValue ? o.PlacedAt.Value.ToString("HH:mm:ss") : null,
                    PickupDate = o.PickupAt.HasValue ? o.PickupAt.Value.ToString("yyyy-MM-dd") : null,
                    PickupTime = o.PickupAt.HasValue ? o.PickupAt.Value.ToString("HH:mm:ss") : null
                };

                if (o.Customer != null)
                {
                    dto.Customer = new CustomerDTO
                    {
                        CustomerId = o.Customer.CustomerId,
                        FirstName = o.Customer.FirstName,
                        LastName = o.Customer.LastName,
                        Email = o.Customer.Email,
                        Username = o.Customer.Username
                    };

                    if (o.Customer.Address != null)
                    {
                        dto.Customer.Address = new AddressDTO
                        {
                            HouseNo = o.Customer.Address.HouseNo,
                            Street = o.Customer.Address.Street,
                            City = o.Customer.Address.City,
                            PostalCode = o.Customer.Address.PostalCode,
                            FullAddress = $"{o.Customer.Address.HouseNo ?? ""}, {o.Customer.Address.Street ?? ""}, {o.Customer.Address.City ?? ""}, {o.Customer.Address.PostalCode ?? ""}"
                        };
                    }
                }

                if (o.Laundry != null)
                {
                    dto.Laundry = new LaundryDTO
                    {
                        LaundryId = o.Laundry.LaundryId,
                        LaundryName = o.Laundry.LaundryName
                    };
                }

                if (o.Status != null)
                {
                    dto.Status = new StatusDTO
                    {
                        StatusID = o.Status.StatusID,
                        StatusName = o.Status.StatusName
                    };
                }

                result.Add(dto);
            }

            return result;
        }

        public async Task<List<OrderDTO>> GetAllOrdersAsync(Guid laundryId)
        {
            // If an empty GUID is passed, return all orders regardless of laundryId
            IQueryable<Order> query = _context.Orders;
            
            if (laundryId != Guid.Empty)
            {
                query = query.Where(o => o.LaundryId == laundryId);
            }

            var orders = await query
                .Include(o => o.Customer)
                .ThenInclude(c => c.Address)
                .Include(o => o.Laundry)
                .Include(o => o.Status)
                .ToListAsync();

            var result = new List<OrderDTO>();
            foreach (var o in orders)
            {
                var dto = new OrderDTO
                {
                    OrderId = o.OrderId,
                    PlacedDate = o.PlacedAt.HasValue ? o.PlacedAt.Value.ToString("yyyy-MM-dd") : null,
                    PlacedTime = o.PlacedAt.HasValue ? o.PlacedAt.Value.ToString("HH:mm:ss") : null,
                    PickupDate = o.PickupAt.HasValue ? o.PickupAt.Value.ToString("yyyy-MM-dd") : null,
                    PickupTime = o.PickupAt.HasValue ? o.PickupAt.Value.ToString("HH:mm:ss") : null
                };

                if (o.Customer != null)
                {
                    dto.CustomerFName = o.Customer.FirstName;
                    dto.CustomerLName = o.Customer.LastName;
                    
                    dto.Customer = new CustomerDTO
                    {
                        CustomerId = o.Customer.CustomerId,
                        FirstName = o.Customer.FirstName,
                        LastName = o.Customer.LastName,
                        Email = o.Customer.Email,
                        Username = o.Customer.Username
                    };

                    if (o.Customer.Address != null)
                    {
                        dto.Customer.Address = new AddressDTO
                        {
                            HouseNo = o.Customer.Address.HouseNo,
                            Street = o.Customer.Address.Street,
                            City = o.Customer.Address.City,
                            PostalCode = o.Customer.Address.PostalCode,
                            FullAddress = $"{o.Customer.Address.HouseNo ?? ""}, {o.Customer.Address.Street ?? ""}, {o.Customer.Address.City ?? ""}, {o.Customer.Address.PostalCode ?? ""}"
                        };
                    }
                }

                if (o.Laundry != null)
                {
                    dto.Laundry = new LaundryDTO
                    {
                        LaundryId = o.Laundry.LaundryId,
                        LaundryName = o.Laundry.LaundryName
                    };
                }

                if (o.Status != null)
                {
                    dto.StatusName = o.Status.StatusName;
                    
                    dto.Status = new StatusDTO
                    {
                        StatusID = o.Status.StatusID,
                        StatusName = o.Status.StatusName
                    };
                }

                result.Add(dto);
            }

            return result;
        }
        
        public async Task<bool> UpdateOrderAsync(Guid id, OrderDTO orderDto)
        {
            if (orderDto == null || id != orderDto.OrderId)
            {
                return false;
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return false;
            }

            // Parse dates if provided
            if (!string.IsNullOrEmpty(orderDto.PlacedDate) && !string.IsNullOrEmpty(orderDto.PlacedTime))
            {
                if (DateTime.TryParse($"{orderDto.PlacedDate} {orderDto.PlacedTime}", out DateTime placedDateTime))
                {
                    order.PlacedAt = placedDateTime;
                }
            }

            if (!string.IsNullOrEmpty(orderDto.PickupDate) && !string.IsNullOrEmpty(orderDto.PickupTime))
            {
                if (DateTime.TryParse($"{orderDto.PickupDate} {orderDto.PickupTime}", out DateTime pickupDateTime))
                {
                    order.PickupAt = pickupDateTime;
                }
            }

            // Update foreign keys if new values are provided in the DTO
            if (orderDto.Customer != null && orderDto.Customer.CustomerId != Guid.Empty)
            {
                order.CustomerId = orderDto.Customer.CustomerId;
            }

            if (orderDto.Laundry != null && orderDto.Laundry.LaundryId != Guid.Empty)
            {
                order.LaundryId = orderDto.Laundry.LaundryId;
            }

            if (orderDto.Status != null && orderDto.Status.StatusID != Guid.Empty)
            {
                order.StatusId = orderDto.Status.StatusID;
            }

            _context.Entry(order).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return await OrderExistsAsync(id);
            }
        }
        
        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return false;
            }

            // Check if there are any related OrderDetails and delete them first
            var orderDetails = await _context.OrderDetails.Where(od => od.OrderId == id).ToListAsync();
            if (orderDetails.Any())
            {
                _context.OrderDetails.RemoveRange(orderDetails);
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
        
        private async Task<bool> OrderExistsAsync(Guid id)
        {
            return await _context.Orders.AnyAsync(o => o.OrderId == id);
        }
    }
}