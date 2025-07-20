using Amazon.S3;
using Amazon.S3.Model;
using FreshlyBackendNew.Services.Interfaces;

namespace FreshlyBackendNew.Services.Implementations
{
    public class S3StorageService : IFileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly ILogger<S3StorageService> _logger;
        private readonly string _bucketName;
        private readonly string _folderName;

        public S3StorageService(IAmazonS3 s3Client, IConfiguration configuration, ILogger<S3StorageService> logger)
        {
            _s3Client = s3Client;
            _logger = logger;
            _bucketName = configuration["AWS:S3:BucketName"] ?? throw new ArgumentNullException("AWS:S3:BucketName");
            _folderName = configuration["AWS:S3:FolderName"] ?? "web_application3_images";
        }

        // ✅ CREATE - Upload a new image and return its public URL
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("No file was provided");

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var key = string.IsNullOrEmpty(_folderName) ? fileName : $"{_folderName}/{fileName}";

                using var stream = file.OpenReadStream();
                var request = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key,
                    InputStream = stream,
                    ContentType = file.ContentType,
                    CannedACL = S3CannedACL.PublicRead
                };

                var response = await _s3Client.PutObjectAsync(request);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    var url = $"https://{_bucketName}.s3.{GetRegionFromClient()}.amazonaws.com/{key}";
                    _logger.LogInformation("Uploaded to S3: {Key}", key);
                    return url;
                }

                _logger.LogError("S3 upload failed with status: {StatusCode}", response.HttpStatusCode);
                throw new Exception($"Upload failed. Status: {response.HttpStatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                throw;
            }
        }

        // ✅ READ - Generate and return the public URL of an image
        public Task<string> GetImageUrlAsync(string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    return Task.FromResult<string>(null);

                var key = string.IsNullOrEmpty(_folderName) ? fileName : $"{_folderName}/{fileName}";
                var url = $"https://{_bucketName}.s3.{GetRegionFromClient()}.amazonaws.com/{key}";

                _logger.LogInformation("Generated S3 URL for: {Key}", key);
                return Task.FromResult(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating image URL");
                return Task.FromResult<string>(null);
            }
        }

        // ✅ UPDATE - Replace an existing image with a new one and return the new URL
        public async Task<string> UpdateImageAsync(string existingFileNameOrUrl, IFormFile newFile)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(existingFileNameOrUrl))
                    throw new ArgumentException("Invalid filename or URL");

                if (newFile == null || newFile.Length == 0)
                    throw new ArgumentException("No new file provided");

                // Delete the old image
                await DeleteImageAsync(existingFileNameOrUrl);

                // Upload the new one
                var newUrl = await UploadImageAsync(newFile);

                return newUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating image");
                throw;
            }
        }

        // ✅ DELETE - Remove an image from S3 by key or URL
        public async Task<bool> DeleteImageAsync(string key)
        {
            try
            {
                // If a full URL is passed, extract the key
                if (key.StartsWith("http"))
                {
                    var uri = new Uri(key);
                    key = uri.AbsolutePath.TrimStart('/');
                }

                var request = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                var response = await _s3Client.DeleteObjectAsync(request);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    _logger.LogInformation("Deleted from S3: {Key}", key);
                    return true;
                }

                _logger.LogError("Failed to delete from S3. Status: {StatusCode}", response.HttpStatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image");
                return false;
            }
        }

        // ✅ Helper - Get region from the S3 client configuration
        private string GetRegionFromClient()
        {
            return _s3Client.Config.RegionEndpoint?.SystemName ?? "eu-north-1";
        }
    }
}
