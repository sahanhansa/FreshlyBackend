using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDTO>> GetNewOrdersAsync(Guid laundryId);
        Task<List<OrderDTO>> GetProcessingOrdersAsync(Guid laundryId);
        Task<List<OrderDTO>> GetAllOrdersAsync(Guid laundryId);
        Task<bool> UpdateOrderAsync(Guid id, OrderDTO orderDto);
        Task<bool> DeleteOrderAsync(Guid id);
        Task<bool> ConfirmOrderAsync(ConfirmOrderDTO dto);

    }
}