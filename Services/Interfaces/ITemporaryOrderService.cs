using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface ITemporaryOrderService
    {
        Task<Guid> AddToBasketAsync(AddToBasketDTO dto); // keep your existing method
        Task<List<TemporaryOrderSummaryDTO>> GetCustomerTemporaryOrderSummariesAsync(Guid customerId);
        Task<bool> DeleteItemFromTemporaryOrderAsync(Guid temporaryOrderId, Guid itemId, Guid serviceId);
        Task<bool> DeleteTemporaryOrderAsync(Guid temporaryOrderId);


    }

}
