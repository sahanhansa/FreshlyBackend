using Microsoft.AspNetCore.Http;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IFileStorageService
    {
        // CREATE - Upload a new image and return its public URL
        Task<string> UploadImageAsync(IFormFile file);

        // READ - Get the public URL of an image
        Task<string> GetImageUrlAsync(string fileName);

        // UPDATE - Replace an existing image and return the new public URL
        Task<string> UpdateImageAsync(string existingFileNameOrUrl, IFormFile newFile);

        // DELETE - Remove an image from storage
        Task<bool> DeleteImageAsync(string fileNameOrUrl);
    }
}