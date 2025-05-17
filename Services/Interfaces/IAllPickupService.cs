using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services
{
    public interface IAllPickupService
    {
        public Task<List<PickupDetailsDto>> GetAllPickups();

        public Task<PickupDetailsByIdDto> GetPickupDetailsBYId(string orderID);
    }
}
