using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDTO>> GetNewOrdersAsync(Guid laundryId);
        Task<List<OrderDTO>> GetProcessingOrdersAsync(Guid laundryId);
        Task<List<OrderDTO>> GetAllOrdersAsync(Guid laundryId);
     
    }
}