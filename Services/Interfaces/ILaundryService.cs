using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface ILaundryService
    {
        // Retrieves a list of laundries with their details for customers
        Task<List<LaundryWithAddressDTO>> GetLaundriesForCustomerAsync();

        // Retrieves a list of laundries with admin details
        Task<List<LaundryAdminDTO>> GetLaundriesForAdminAsync();

        // Creates a new laundry
        Task<LaundryAdminDTO> CreateLaundryAsync(LaundryAdminDTO laundryDto);

        // Retrieves a laundry by ID
        Task<LaundryAdminDTO> GetLaundryByIdAsync(Guid id);
    }
}
