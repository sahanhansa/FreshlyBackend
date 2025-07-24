using FreshlyBackendNew.DTOs.Driver_DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IDriverProfileService
    {
        Task<ProfileDetailsByIdDto> GetDriverProfileDetailsAsync(Guid driverId);
        Task<DriverEditDto> GetDriverEdit(Guid driverId);
        Task<DriverHomaDto> DriverHomePage(Guid driverId);
        Task<string> UpdateProfile(DriverEditDto dto);
        Task<string> UpdatePassword(UpdatePasswordDto dto); 
        Task<DriverReportDto> DriverReportDash(Guid driverId);
        Task<decimal> DriverReportRevenue(Guid driverId);
        Task<byte[]> GeneratePdfReport(Guid orderId);
    }
}
