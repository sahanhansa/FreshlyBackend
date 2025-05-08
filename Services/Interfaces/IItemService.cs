using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IItemService
    {
        Task<List<ItemWithServicesDTO>> GetItemsByLaundryIdAsync(Guid laundryId);
    }
}
