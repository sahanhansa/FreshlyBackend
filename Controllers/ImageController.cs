using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;

        public ImageController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        // ✅ CREATE - Upload an image
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file provided or file is empty.");

                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                    return BadRequest("Invalid file type. Only JPEG, PNG, and GIF images are allowed.");

                var imageUrl = await _fileStorageService.UploadImageAsync(file);

                return Ok(new
                {
                    Message = "Image uploaded successfully",
                    ImageUrl = imageUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Image upload failed", Error = ex.Message });
            }
        }

        // ✅ READ - Get image by filename
        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetImage(string fileName)
        {
            try
            {
                var imageUrl = await _fileStorageService.GetImageUrlAsync(fileName);

                if (string.IsNullOrEmpty(imageUrl))
                    return NotFound("Image not found.");

                return Ok(new
                {
                    Message = "Image retrieved successfully",
                    ImageUrl = imageUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to retrieve image", Error = ex.Message });
            }
        }

        // ✅ UPDATE - Replace image with new one
        [HttpPut("update/{fileName}")]
        public async Task<IActionResult> UpdateImage(string fileName, IFormFile newFile)
        {
            try
            {
                if (newFile == null || newFile.Length == 0)
                    return BadRequest("No new image provided.");

                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(newFile.ContentType.ToLower()))
                    return BadRequest("Invalid file type.");

                // Delete old image
                await _fileStorageService.DeleteImageAsync(fileName);

                // Upload new image
                var newImageUrl = await _fileStorageService.UploadImageAsync(newFile);

                return Ok(new
                {
                    Message = "Image updated successfully",
                    ImageUrl = newImageUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Image update failed", Error = ex.Message });
            }
        }

        // ✅ DELETE - Remove image by filename
        [HttpDelete("delete/{fileName}")]
        public async Task<IActionResult> DeleteImage(string fileName)
        {
            try
            {
                await _fileStorageService.DeleteImageAsync(fileName);
                return Ok(new { Message = "Image deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Image deletion failed", Error = ex.Message });
            }
        }
    }
}
