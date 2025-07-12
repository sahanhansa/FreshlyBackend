using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface ITemporaryOrderService
    {
        Task<Guid> AddToBasketAsync(AddToBasketDTO dto);
    }

}
