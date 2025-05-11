using FreshlyBackendNew.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Services
{
    public interface IOrderService
    {
        // Get all orders with basic details
        Task<List<OrderDTO>> GetAllOrdersAsync();

        // Get detailed order information
        Task<List<OrderDTO>> GetOrderDetailsAsync();

        // Get a specific order by ID
        Task<OrderDTO> GetOrderByIdAsync(Guid id);

        // Create a new order
        Task<OrderDTO> CreateOrderAsync(OrderDTO orderDto);

        // Update an existing order
        Task<bool> UpdateOrderAsync(Guid id, OrderDTO orderDto);

        // Delete an order
        Task<bool> DeleteOrderAsync(Guid id);
    }
}