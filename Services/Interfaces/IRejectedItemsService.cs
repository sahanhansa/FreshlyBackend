using FreshlyBackendNew.DTOs;
using System;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IRejectedItemService
    {
        Task<RejectedItemDTO?> GetRejectedItemByIdAsync(Guid rejectedItemId);
        Task<RejectedItemDTO> AddRejectedItemAsync(RejectedItemDTO rejectedItemDto);
        Task<IEnumerable<RejectedItemDTO>> GetRejectedItemsByLaundryIdAsync(Guid laundryId);
        Task<List<RejectedItemWithGarmentDTO>> GetRejectedItemsByOrderIdAsync(Guid orderId);

    }
}