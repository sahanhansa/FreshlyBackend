using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Fix for CS1061: Replace 'PlacedDate' and 'PlacedTime' with 'PlacedAt' and adjust the logic accordingly.
        // Similarly, replace 'PickupDate' and 'PickupTime' with 'PickupAt' and adjust the logic.

        public async Task<List<OrderDTO>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                    PlacedDate = o.PlacedAt.HasValue ? o.PlacedAt.Value.ToString("yyyy-MM-dd") : null,
                    PlacedTime = o.PlacedAt.HasValue ? o.PlacedAt.Value.ToString("HH:mm:ss") : null,
                    PickupDate = o.PickupAt.HasValue ? o.PickupAt.Value.ToString("yyyy-MM-dd") : null,
                    PickupTime = o.PickupAt.HasValue ? o.PickupAt.Value.ToString("HH:mm:ss") : null
                })
                .ToListAsync();
        }

        // Get detailed order information
        public async Task<List<OrderDTO>> GetOrderDetailsAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer).ThenInclude(c => c.Address)
                .Include(o => o.Laundry)
                .Include(o => o.Status)
                .Include(o => o.OrderType)
                .Include(o => o.User)
                .Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                    PlacedDate = o.PlacedAt.HasValue ? o.PlacedAt.Value.ToString("yyyy-MM-dd") : null,
                    PlacedTime = o.PlacedAt.HasValue ? o.PlacedAt.Value.ToString("HH:mm:ss") : null,
                    PickupDate = o.PickupAt.HasValue ? o.PickupAt.Value.ToString("yyyy-MM-dd") : null,
                    PickupTime = o.PickupAt.HasValue ? o.PickupAt.Value.ToString("HH:mm:ss") : null,
                    Customer = o.Customer != null ? new CustomerDTO
                    {
                        CustomerId = o.Customer.CustomerId,
                        FirstName = o.Customer.FirstName,
                        LastName = o.Customer.LastName,
                        Email = o.Customer.Email,
                        Username = o.Customer.Username,
                        Address = o.Customer.Address != null ? new AddressDTO
                        {
                            HouseNo = o.Customer.Address.HouseNo,
                            Street = o.Customer.Address.Street,
                            City = o.Customer.Address.City,
                            PostalCode = o.Customer.Address.PostalCode,
                            FullAddress = o.Customer.Address != null
                                ? $"{o.Customer.Address.HouseNo}, {o.Customer.Address.Street}, {o.Customer.Address.City}, {o.Customer.Address.PostalCode}"
                                : null
                        } : null
                    } : null,
                    Laundry = o.Laundry != null ? new LaundryDTO
                    {
                        LaundryId = o.Laundry.LaundryId,
                        LaundryName = o.Laundry.LaundryName
                    } : null,
                    Status = o.Status != null ? new StatusDTO
                    {
                        StatusID = o.Status.StatusID,
                        StatusName = o.Status.StatusName
                    } : null,
                    OrderType = o.OrderType != null ? new OrderTypeDTO
                    {
                        TypeId = o.OrderType.TypeId,
                        TypeName = o.OrderType.TypeName
                    } : null,
                    User = o.User != null ? new UserDTO
                    {
                        UserId = o.User.UserId,
                        Username = o.User.Username
                    } : null
                })
                .ToListAsync();
        }

        // Get a specific order by ID
        public async Task<OrderDTO> GetOrderByIdAsync(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer).ThenInclude(c => c.Address)
                .Include(o => o.Laundry)
                .Include(o => o.Status)
                .Include(o => o.OrderType)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return null;
            }

            return new OrderDTO
            {
                OrderId = order.OrderId,
                PlacedDate = order.PlacedAt.HasValue ? order.PlacedAt.Value.ToString("yyyy-MM-dd") : null,
                PlacedTime = order.PlacedAt.HasValue ? order.PlacedAt.Value.ToString("HH:mm:ss") : null,
                PickupDate = order.PickupAt.HasValue ? order.PickupAt.Value.ToString("yyyy-MM-dd") : null,
                PickupTime = order.PickupAt.HasValue ? order.PickupAt.Value.ToString("HH:mm:ss") : null,
                Customer = order.Customer != null ? new CustomerDTO
                {
                    CustomerId = order.Customer.CustomerId,
                    FirstName = order.Customer.FirstName,
                    LastName = order.Customer.LastName,
                    Email = order.Customer.Email,
                    Username = order.Customer.Username,
                    Address = order.Customer.Address != null ? new AddressDTO
                    {
                        HouseNo = order.Customer.Address.HouseNo,
                        Street = order.Customer.Address.Street,
                        City = order.Customer.Address.City,
                        PostalCode = order.Customer.Address.PostalCode,
                        FullAddress = order.Customer.Address != null
                            ? $"{order.Customer.Address.HouseNo}, {order.Customer.Address.Street}, {order.Customer.Address.City}, {order.Customer.Address.PostalCode}"
                            : null
                    } : null
                } : null,
                Laundry = order.Laundry != null ? new LaundryDTO
                {
                    LaundryId = order.Laundry.LaundryId,
                    LaundryName = order.Laundry.LaundryName
                } : null,
                Status = order.Status != null ? new StatusDTO
                {
                    StatusID = order.Status.StatusID,
                    StatusName = order.Status.StatusName
                } : null,
                OrderType = order.OrderType != null ? new OrderTypeDTO
                {
                    TypeId = order.OrderType.TypeId,
                    TypeName = order.OrderType.TypeName
                } : null,
                User = order.User != null ? new UserDTO
                {
                    UserId = order.User.UserId,
                    Username = order.User.Username
                } : null
            };
        }

        // Create a new order
        public async Task<OrderDTO> CreateOrderAsync(OrderDTO orderDto)
        {
            if (orderDto == null)
            {
                throw new ArgumentNullException(nameof(orderDto));
            }

            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                PlacedDate = orderDto.PlacedDate != null ? DateTime.Parse(orderDto.PlacedDate) : null,
                PlacedTime = orderDto.PlacedTime != null ? DateTime.Parse(orderDto.PlacedTime) : null,
                PickupDate = orderDto.PickupDate != null ? DateTime.Parse(orderDto.PickupDate) : null,
                PickupTime = orderDto.PickupTime != null ? DateTime.Parse(orderDto.PickupTime) : null,
                CustomerId = orderDto.Customer?.CustomerId,
                LaundryId = orderDto.Laundry?.LaundryId,
                StatusId = orderDto.Status?.StatusID,
                TypeId = orderDto.OrderType?.TypeId,
                UserId = orderDto.User?.UserId
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return await GetOrderByIdAsync(order.OrderId);
        }

        // Update an existing order
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

            order.PlacedDate = orderDto.PlacedDate != null ? DateTime.Parse(orderDto.PlacedDate) : null;
            order.PlacedTime = orderDto.PlacedTime != null ? DateTime.Parse(orderDto.PlacedTime) : null;
            order.PickupDate = orderDto.PickupDate != null ? DateTime.Parse(orderDto.PickupDate) : null;
            order.PickupTime = orderDto.PickupTime != null ? DateTime.Parse(orderDto.PickupTime) : null;
            order.CustomerId = orderDto.Customer?.CustomerId;
            order.LaundryId = orderDto.Laundry?.LaundryId;
            order.StatusId = orderDto.Status?.StatusID;
            order.TypeId = orderDto.OrderType?.TypeId;
            order.UserId = orderDto.User?.UserId;

            _context.Entry(order).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return !await OrderExistsAsync(id);
            }
        }

        // Delete an order
        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return false;
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