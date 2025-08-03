using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IBasicService
    {
        Task<List<LaundryDetailsWithImageDTO>> GetItemsByLaundryWithImageAsync();
    }
}
