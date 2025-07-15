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
    public class OrderService : FreshlyBackendNew.Services.Interfaces.IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<OrderDTO>> GetNewOrdersAsync(Guid laundryId)
        {
            var pickedUpStatus = await _context.Statuses
                .FirstOrDefaultAsync(s => s.StatusName != null && s.StatusName.ToLower() == "picked up");

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
        .FirstOrDefaultAsync(s => s.StatusName != null && s.StatusName.ToLower() == "processing in laundry");

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
            PlacedDate = o.PlacedAt?.ToString("yyyy-MM-dd"),
            PlacedTime = o.PlacedAt?.ToString("HH:mm:ss"),
            PickupDate = o.PickupAt?.ToString("yyyy-MM-dd"),
            PickupTime = o.PickupAt?.ToString("HH:mm:ss")
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
                    PickupTime = o.PickupAt.HasValue ? o.PickupAt.Value.ToString("HH:mm:ss") : null,
                    PlacedDateTime = o.PlacedAt
                };

                if (o.Customer != null)
                {
                    dto.Customer = new CustomerDTO
                    {
                        CustomerId = o.Customer.CustomerId,
                        FirstName = o.Customer.FirstName,
                        LastName = o.Customer.LastName,
                        Email = o.Customer.Email,
                        Username = o.Customer.Username,
                        CustomerFName = o.Customer.FirstName ?? string.Empty,
                        CustomerLName = o.Customer.LastName ?? string.Empty
                    };

                    if (o.Customer.Address != null)
                    {
                        dto.Customer.Address = new AddressDTO
                        {
                            AddressId = o.Customer.Address.AddressId,
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
                        StatusName = o.Status.StatusName,
                        StatusDisplayName = o.Status.StatusName ?? string.Empty
                    };
                }

                result.Add(dto);
            }

            return result;
        }
        
        public async Task<OrderDTO> GetOrderByIdAsync(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .ThenInclude(c => c.Address)
                .Include(o => o.Laundry)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return null;

            var dto = new OrderDTO
            {
                OrderId = order.OrderId,
                PlacedDate = order.PlacedAt.HasValue ? order.PlacedAt.Value.ToString("yyyy-MM-dd") : null,
                PlacedTime = order.PlacedAt.HasValue ? order.PlacedAt.Value.ToString("HH:mm:ss") : null,
                PickupDate = order.PickupAt.HasValue ? order.PickupAt.Value.ToString("yyyy-MM-dd") : null,
                PickupTime = order.PickupAt.HasValue ? order.PickupAt.Value.ToString("HH:mm:ss") : null,
                PlacedDateTime = order.PlacedAt
            };

            if (order.Customer != null)
            {
                dto.Customer = new CustomerDTO
                {
                    CustomerId = order.Customer.CustomerId,
                    FirstName = order.Customer.FirstName,
                    LastName = order.Customer.LastName,
                    Email = order.Customer.Email,
                    Username = order.Customer.Username,
                    CustomerFName = order.Customer.FirstName ?? string.Empty,
                    CustomerLName = order.Customer.LastName ?? string.Empty
                };

                if (order.Customer.Address != null)
                {
                    dto.Customer.Address = new AddressDTO
                    {
                        AddressId = order.Customer.Address.AddressId,
                        HouseNo = order.Customer.Address.HouseNo,
                        Street = order.Customer.Address.Street,
                        City = order.Customer.Address.City,
                        PostalCode = order.Customer.Address.PostalCode,
                        FullAddress = $"{order.Customer.Address.HouseNo ?? ""}, {order.Customer.Address.Street ?? ""}, {order.Customer.Address.City ?? ""}, {order.Customer.Address.PostalCode ?? ""}"
                    };
                }
            }

            if (order.Laundry != null)
            {
                dto.Laundry = new LaundryDTO
                {
                    LaundryId = order.Laundry.LaundryId,
                    LaundryName = order.Laundry.LaundryName
                };
            }

            if (order.Status != null)
            {
                dto.Status = new StatusDTO
                {
                    StatusID = order.Status.StatusID,
                    StatusName = order.Status.StatusName,
                    StatusDisplayName = order.Status.StatusName ?? string.Empty
                };
            }

            return dto;
        }
        
        public async Task<OrderDTO> CreateOrderAsync(OrderDTO orderDto)
        {
            if (orderDto == null)
                return null;

            var order = new Order
            {
                OrderId = orderDto.OrderId != Guid.Empty ? orderDto.OrderId : Guid.NewGuid()
            };

            // Parse dates if provided
            if (!string.IsNullOrEmpty(orderDto.PlacedDate) && !string.IsNullOrEmpty(orderDto.PlacedTime))
            {
                if (DateTime.TryParse($"{orderDto.PlacedDate} {orderDto.PlacedTime}", out DateTime placedDateTime))
                {
                    order.PlacedAt = placedDateTime;
                }
            }
            else if (orderDto.PlacedDateTime.HasValue)
            {
                order.PlacedAt = orderDto.PlacedDateTime.Value;
            }
            else
            {
                order.PlacedAt = DateTime.UtcNow;
            }

            if (!string.IsNullOrEmpty(orderDto.PickupDate) && !string.IsNullOrEmpty(orderDto.PickupTime))
            {
                if (DateTime.TryParse($"{orderDto.PickupDate} {orderDto.PickupTime}", out DateTime pickupDateTime))
                {
                    order.PickupAt = pickupDateTime;
                }
            }

            // Set foreign keys from DTO
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
            else
            {
                // Set default status if not provided
                var defaultStatus = await _context.Statuses
                    .FirstOrDefaultAsync(s => s.StatusName != null && 
                        string.Equals(s.StatusName, "placed", StringComparison.OrdinalIgnoreCase));
                
                if (defaultStatus != null)
                {
                    order.StatusId = defaultStatus.StatusID;
                }
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Return the created order
            return await GetOrderByIdAsync(order.OrderId);
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
            else if (orderDto.PlacedDateTime.HasValue)
            {
                order.PlacedAt = orderDto.PlacedDateTime.Value;
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