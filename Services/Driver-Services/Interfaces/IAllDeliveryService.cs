using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services
{
    public interface IAllDeliveryService
    {
        public Task<List<DeliveryDetailsDto>> GetAllDeliveries();

        public Task<DeliveryDetailsByIdDto> GetDeliveryDetailsBYId(string orderID);
    }
}
