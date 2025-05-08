using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface ILaundryService
    {
        // Retrieves a list of laundries with their details
        Task<List<LaundryWithAddressDTO>> GetLaundriesForCustomerAsync();
    }
}
