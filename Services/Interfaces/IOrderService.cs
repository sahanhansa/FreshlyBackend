using FreshlyBackendNew.Common;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.DTOs.Order_DTOs;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDTO>> GetNewOrdersAsync(Guid laundryId);
        Task<List<OrderDTO>> GetProcessingOrdersAsync(Guid laundryId);
        Task<List<OrderDTO>> GetCompletedOrdersAsync(Guid laundryId);
        Task<List<OrderDTO>> GetAllOrdersAsync(Guid laundryId);
        Task<OrderDTO> GetOrderByIdAsync(Guid id);
        Task<Result<OrderDTO>> CreateOrderAsync(OrderDTO orderDto);
        Task<bool> UpdateOrderAsync(Guid id, OrderDTO orderDto);
        Task<bool> DeleteOrderAsync(Guid id);
        
        Task<FreshlyBackendNew.DTOs.Order_DTOs.AddressDTO> GetCustomerAddressAsync(Guid customerId);
        
        Task<bool> ConfirmOrderAsync(ConfirmOrderDTO dto);
        Task<List<OrderDTO>> GetFilteredOrdersAsync(Guid laundryId);
        Task<int> GetOrderCountByStatusAsync(Guid laundryId, Guid statusId);
        Task<SortedOrderIdsResponseDTO> GetSortedOrderIdsAsync(Guid laundryId);
        Task<PaginatedOrderResponseDTO> GetFilteredOrdersPaginatedAsync(Guid laundryId, int pageNumber, int pageSize, string? statusFilter = null, string? searchTerm = null);
        Task<PaginatedOrderResponseDTO> GetAllOrdersPaginatedAsync(Guid laundryId, int pageNumber, int pageSize, string? searchTerm = null);
        Task<int> GetTotalOrderCountAsync(Guid laundryId, string? statusFilter = null);
    }
}