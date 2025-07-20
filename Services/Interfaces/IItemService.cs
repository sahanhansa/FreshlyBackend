using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IItemService
    {
        // Retrieves a list of items by laundry id
        Task<List<ItemWithServicesDTO>> GetItemsByLaundryIdAsync(Guid laundryId);
        
       //Add an item
       Task<bool> AddItemAsync(AddItemDTO itemDto, Guid laundryId);
        
        //Delete an item
        Task<bool> DeleteItemAsync(Guid itemId, Guid laundryId);
   
        
        //Update an item
        Task<bool> UpdateItemAsync(Guid itemId, UpdateItemDTO itemDto, Guid laundryId);

        
      
        
    
      
     

    }
}
