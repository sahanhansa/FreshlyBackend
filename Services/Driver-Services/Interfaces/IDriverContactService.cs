using FreshlyBackendNew.DTOs.Driver_DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IDriverContactService
    {
        Task<DriverContactDetailsDto?> GetDriverContactDetailsAsync(Guid driverId);
        Task AddMessage(DriverContactDetailsDto driverContactDetailsDto);
    }
}
