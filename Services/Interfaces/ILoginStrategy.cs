using FreshlyBackendNew.DTOs;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface ILoginStrategy
    {
        Task<AuthResponse?> LoginAsync(LoginData loginData);
    }
}