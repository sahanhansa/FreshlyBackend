using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface ILaundryService
    {
        // Retrieves a list of laundries with their details for customer view
        Task<List<LaundryWithAddressDTO>> GetLaundriesForCustomerAsync();
        
        // Retrieves a list of laundries with additional details for admin view
        Task<List<LaundryAdminDTO>> GetLaundriesForAdminAsync();
        
        // Creates a new laundry from the provided DTO
        Task<LaundryAdminDTO> CreateLaundryAsync(LaundryAdminDTO laundryDto);
        
        // Retrieves a specific laundry by its ID
        Task<LaundryAdminDTO> GetLaundryByIdAsync(Guid id);
        
        // Retrieves laundry details with image for a specific laundry
        Task<LaundryDetailsDTO> GetLaundryDetailsAsync(Guid laundryId);
        
        
    }
}
