using FreshlyBackendNew.DTOs.Driver_DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IDriverProfileService
    {
        Task<ProfileDetailsByIdDto> GetDriverProfileDetailsAsync(Guid driverId);
        Task<DriverEditDto> GetDriverEdit(Guid driverId);
        Task<DriverHomaDto> DriverHomePage(Guid driverId);
    }
}
