using FreshlyBackendNew.DTOs.Driver_DTOs;
using System;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IDriverContactService
    {
        Task<DriverContactDetailsDto?> GetDriverContactDetailsAsync(Guid driverId);
        Task AddMessage(DriverContactDetailsDto driverContactDetailsDto); // Added for DriverController
    }
}
