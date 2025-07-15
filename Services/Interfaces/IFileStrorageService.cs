namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task<bool> DeleteImageAsync(string key);
    }
}