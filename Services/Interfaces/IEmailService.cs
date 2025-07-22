using System.Threading.Tasks;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
