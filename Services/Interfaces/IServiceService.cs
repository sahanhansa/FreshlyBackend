using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Implementations;

 public interface IServiceService
{
    Task<ServiceIdResponseDTO> GetServiceIdByNameAsync(string serviceName);
}