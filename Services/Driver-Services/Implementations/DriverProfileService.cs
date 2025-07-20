using FreshlyBackendNew.DTOs.Driver_DTOs;
using FreshlyBackendNew.Data;
using Microsoft.EntityFrameworkCore;
using FreshlyBackendNew.Services.Interfaces;

namespace FreshlyBackendNew.Services.Implementations
{
    public class DriverProfileService : IDriverProfileService
    {
        private readonly ApplicationDbContext _context;

        public DriverProfileService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProfileDetailsByIdDto?> GetDriverProfileDetailsAsync(Guid driverId)
        {
            var driver = await _context.Drivers
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DriverId == driverId);

            if (driver == null)
                return null;

            var dto = new ProfileDetailsByIdDto
            {
                DriverID = driver.DriverId,
                FirstName = driver.FirstName ?? "",
                LastName = driver.LastName ?? "",
                LicenseNumber = driver.LicenseNo ?? "",
                Email = driver.Email ?? "",
                //ContactNumber = driver.ContactNumber ?? "",
                //HomeAddress = driver.HomeAddress ?? "",
                //VehicleNumber = driver.VehicleNumber ?? "",
                //Location = driver.Location ?? ""
            };

            return dto;
        }
    }
}
