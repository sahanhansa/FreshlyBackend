using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services
{
    public interface IAllDeliveryService
    {
        public Task<List<DeliveryDetailsDto>> GetAllDeliveries();

        public Task<DeliveryDetailsByIdDto> GetDeliveryDetailsBYId(string orderID);
        public Task MarksToLaundryTake(MarkOrderDto markOrderDto);
        public Task MarksToLaundryPick(MarkOrderDto markOrderDto);
        public Task MarksToCustomerDeliver(MarkOrderDto markOrderDto);
    }
}
