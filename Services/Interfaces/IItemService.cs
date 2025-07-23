using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IItemService
    {
        // Retrieves a list of items by laundry id
        Task<List<ItemWithServicesDTO>> GetItemsByLaundryIdAsync(Guid laundryId);
        
        // Retrieves a single item by laundry id and item id
        Task<ItemWithServicesDTO> GetItemByLaundryIdAsync(Guid laundryId, Guid itemId);
        
        // Retrieves a single item with services by laundry id and item id
        Task<ItemWithGarmentTypesDTO?> GetItemByLaundryIdAndItemIdAsync(Guid laundryId, Guid itemId);
       //Add an item
       Task<(bool success, string message)> AddItemAsync(AddItemDTO itemDto, Guid laundryId);
       
       Task<(bool success, string message)> AddGarmentTypeAsync(AddGarmentTypeDTO garmentTypeDto);
       
   

        Task<List<ItemWithServicesDTO>> GetItemsByLaundryIdAsync(Guid laundryId, Guid garmentTypeId);



        //Delete an item
        Task<bool> DeleteItemAsync(Guid itemId, Guid laundryId);
   
        
        //Update an item
        Task<bool> UpdateItemAsync(Guid itemId, UpdateItemDTO itemDto, Guid laundryId);

        //Get item image URL for deletion
        Task<string> GetItemImageUrlAsync(Guid itemId, Guid laundryId);

        Task<Guid?> GetGarmentTypeIdByNameAsync(string name);
        
      
        
    
      
     

    }
}
