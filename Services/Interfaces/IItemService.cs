using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IItemService
    {
        // Retrieves a list of items by laundry id
        Task<List<ItemWithServicesDTO>> GetItemsByLaundryIdAsync(Guid laundryId);
        
        //Delete an item
        Task<bool> DeleteItemAsync(Guid id);
    }
}
