using FreshlyBackendNew.DTOs.Driver_DTOs;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Data;
using Microsoft.EntityFrameworkCore;
using FreshlyBackendNew.Services.Interfaces;

namespace FreshlyBackendNew.Services.Implementations
{
    public class DriverContactService : IDriverContactService
    {
        private readonly ApplicationDbContext _context;

        public DriverContactService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DriverContactDetailsDto?> GetDriverContactDetailsAsync(Guid driverId)
        {
            var driver = await _context.Drivers
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DriverId == driverId);

            if (driver == null)
                return null;

            var dto = new DriverContactDetailsDto
            {
                DriverID = driver.DriverId,
                FirstName = driver.FirstName ?? "",
                LastName = driver.LastName ?? "",
                Email = driver.Email ?? ""
            };

            return dto;
        }

        public async Task AddMessage(DriverContactDetailsDto driverContactDetailsDto)
        {
            var message = new Feedback
            {
                Description = driverContactDetailsDto.Message,
                SubmittedByType="driver",
                InquiryType=driverContactDetailsDto.SelectedSubject,
                UserId=driverContactDetailsDto.DriverID


            };

            _context.Feedbacks.Add(message);
            await _context.SaveChangesAsync();
        }

    }
}
