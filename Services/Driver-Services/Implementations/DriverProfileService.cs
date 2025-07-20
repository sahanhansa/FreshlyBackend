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
                VehicleNumber = driver.VehicleNo ?? "",
                //ContactNumber = driver.ContactNumber ?? "",
                //HomeAddress = driver.HomeAddress ?? "",
                //VehicleNumber = driver.VehicleNumber ?? "",
                //Location = driver.Location ?? ""
            };

            var contacts = await _context.Contacts
                .Where(d => d.UserId == dto.DriverID)
                .Select(d => d.ContactNumber)
                .ToListAsync();

            var address = await _context.Addresses
                .Where(c => c.AddressId == driver.AddressId)
                 .Select(d => new
                 {
                     d.HouseNo,
                     d.Street,
                     d.City,
                     d.PostalCode
                 })
                .FirstOrDefaultAsync();


            dto.ContactNumber = contacts.ToArray();
            dto.HomeAddress = $"{address.HouseNo}, {address.Street}, {address.City}";
            dto.Location = address.City;

            return dto;
        }

        public async Task<DriverEditDto> GetDriverEdit(Guid driverId)
        {
            var driver = await _context.Drivers
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DriverId == driverId);

            if (driver == null)
                return null;

            var dto = new DriverEditDto
            {
                DriverID = driver.DriverId,
                FirstName = driver.FirstName ?? "",
                LastName = driver.LastName ?? "",
                Email = driver.Email ?? "",
            };

            var contacts = await _context.Contacts
                .Where(d => d.UserId == dto.DriverID)
                .Select(d => d.ContactNumber)
                .ToListAsync();

            var address = await _context.Addresses
                .Where(c => c.AddressId == driver.AddressId)
                 .Select(d => new
                 {
                     d.HouseNo,
                     d.Street,
                     d.City,
                     d.PostalCode
                 })
                .FirstOrDefaultAsync();


            dto.ContactNumber = contacts.ToArray();
            dto.HouseNo = address.HouseNo;
            dto.Street = address.Street;
            dto.City = address.City;

            return dto;
        }
    }
}
