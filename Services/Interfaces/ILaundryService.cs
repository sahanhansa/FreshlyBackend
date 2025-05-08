using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface ILaundryService
    {
        Task<List<LaundryWithAddressDTO>> GetLaundriesForCustomerAsync();
    }
}
