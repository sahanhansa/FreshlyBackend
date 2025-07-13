using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<List<FeedbackDTO>> GetFeedbacksAsync(Guid laundryId);
    }
}