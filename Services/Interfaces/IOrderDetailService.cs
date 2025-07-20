using FreshlyBackendNew.DTOs.Order_DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IOrderDetailService
    {
        //ongoing
        Task<OrderDetailsDTO> GetOrderDetailsAsync(Guid orderId);
        Task<List<OrderDetailsDTO>> GetOngoingOrdersForCustomerAsync(Guid customerId);
        Task<decimal> CalculateOrderTotalAsync(Guid orderId);
        Task<bool> CancelOrderAsync(Guid orderId);

        //topay
        Task<List<OrderDetailsDTO>> GetOutForDeliveryOrdersForCustomerAsync(Guid customerId);

    }
}