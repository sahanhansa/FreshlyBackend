using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Services.Implementations
{
    public class ServicesService : IServiceService
    {
        private readonly ApplicationDbContext _context;

        public ServicesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceIdResponseDTO?> GetServiceIdByNameAsync(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                return null;

            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.ServiceName.ToLower() == serviceName.ToLower());

            if (service == null)
                return null;

            return new ServiceIdResponseDTO
            {
                ServiceId = service.ServiceId
            };
        }
    }
}