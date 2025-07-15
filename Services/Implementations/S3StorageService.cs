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

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("No file was provided");
                }

                // Generate unique file name
                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var key = string.IsNullOrEmpty(_folderName) ? fileName : $"{_folderName}/{fileName}";

                using var stream = file.OpenReadStream();
                
                var request = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key,
                    InputStream = stream,
                    ContentType = file.ContentType,
                    CannedACL = S3CannedACL.PublicRead // Make the object publicly readable
                };

                var response = await _s3Client.PutObjectAsync(request);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Return the public URL
                    var url = $"https://{_bucketName}.s3.{GetRegionFromClient()}.amazonaws.com/{key}";
                    _logger.LogInformation("Successfully uploaded file to S3: {Key}", key);
                    return url;
                }
                else
                {
                    _logger.LogError("S3 upload failed with status: {StatusCode}", response.HttpStatusCode);
                    throw new Exception($"Failed to upload image to S3. Status: {response.HttpStatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image to S3");
                throw;
            }
        }

        public async Task<bool> DeleteImageAsync(string key)
        {
            try
            {
                // If a full URL is passed, extract just the key
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
                    _logger.LogInformation("Successfully deleted file from S3: {Key}", key);
                    return true;
                }
                else
                {
                    _logger.LogError("S3 delete failed with status: {StatusCode}", response.HttpStatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image from S3");
                return false;
            }
        }

        private string GetRegionFromClient()
        {
            // Extract region from the S3 client configuration
            // Default to eu-north-1 if not available
            return _s3Client.Config.RegionEndpoint?.SystemName ?? "eu-north-1";
        }
    }
} 