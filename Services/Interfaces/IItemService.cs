using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IItemService
    {
        // Retrieves a list of items by laundry id
        Task<List<ItemWithServicesDTO>> GetItemsByLaundryIdAsync(Guid laundryId);
        
       //Add an item
        Task<bool> AddItemAsync(AddItemDTO itemDto);
        
        //Delete an item
        Task<bool> DeleteItemAsync(Guid id);
    }
}
