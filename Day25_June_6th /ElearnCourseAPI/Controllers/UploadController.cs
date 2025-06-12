using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElearnAPI.Controllers
{
    [ApiController]
    [Route("api/v1/files")]
    public class UploadController : ControllerBase
    {
        private readonly IUploadService _uploadService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly Serilog.ILogger _logger;
        private readonly string _uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        public UploadController(IUploadService uploadService, IHubContext<NotificationHub> hubContext)
        {
            _uploadService = uploadService;
            _hubContext = hubContext;
            _logger = Log.ForContext<UploadController>();
        }

        [Authorize(Roles = "Instructor")]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadFileDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdClaim, out var uploadedBy))
                {
                    _logger.Warning("Invalid token during upload.");
                    return Unauthorized(new { success = false, message = "Invalid token. Cannot extract user ID." });
                }

                if (!Directory.Exists(_uploadRoot))
                    Directory.CreateDirectory(_uploadRoot);

                var sanitizedFileName = Path.GetFileName(dto.File.FileName);
                var filePath = Path.Combine(_uploadRoot, sanitizedFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                var fileMeta = await _uploadService.UploadFileAsync(new UploadedFileDto
                {
                    FileName = sanitizedFileName,
                    Path = filePath,
                    CourseId = dto.CourseId,
                });

                _logger.Information("File uploaded: {FileName} for Course: {CourseId}", sanitizedFileName, dto.CourseId);

                await _hubContext.Clients.Group($"course-{dto.CourseId}")
                    .SendAsync("ReceiveNotification", new
                    {
                        message = $"📂 New file uploaded: {sanitizedFileName}",
                        courseId = dto.CourseId,
                        uploadedAt = DateTime.UtcNow
                    });

                return Ok(new { success = true, data = fileMeta });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "File upload failed.");
                return StatusCode(500, new
                {
                    success = false,
                    message = "File upload failed.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("{filename}")]
        public IActionResult Download(string filename)
        {
            var sanitizedFileName = Path.GetFileName(filename);
            var filePath = Path.Combine(_uploadRoot, sanitizedFileName);

            if (!System.IO.File.Exists(filePath))
            {
                _logger.Warning("Download failed. File not found: {FileName}", sanitizedFileName);
                return NotFound(new { success = false, message = "File not found" });
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            _logger.Information("File downloaded: {FileName}", sanitizedFileName);
            return File(fileBytes, "application/octet-stream", sanitizedFileName);
        }

        [Authorize(Roles = "Instructor")]
        [HttpPut("{fileId:guid}")]
        public async Task<IActionResult> Update(Guid fileId, [FromForm] UploadFileDto dto)
        {
            try
            {
                var existingFile = await _uploadService.GetFileByIdAsync(fileId);
                if (existingFile == null)
                {
                    _logger.Warning("Update failed. File not found: {FileId}", fileId);
                    return NotFound(new { success = false, message = "File not found." });
                }

                if (!Directory.Exists(_uploadRoot))
                    Directory.CreateDirectory(_uploadRoot);

                if (System.IO.File.Exists(existingFile.Path))
                    System.IO.File.Delete(existingFile.Path);

                var sanitizedFileName = Path.GetFileName(dto.File.FileName);
                var newPath = Path.Combine(_uploadRoot, sanitizedFileName);

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                existingFile.FileName = sanitizedFileName;
                existingFile.Path = newPath;
                existingFile.CourseId = dto.CourseId;

                await _uploadService.UploadFileAsync(new UploadedFileDto
                {
                    FileName = sanitizedFileName,
                    Path = newPath,
                    CourseId = dto.CourseId
                });

                _logger.Information("File updated: {FileName} for FileId: {FileId}", sanitizedFileName, fileId);
                return Ok(new { success = true, message = "File updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "File update failed for FileId: {FileId}", fileId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "File update failed.",
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "Instructor")]
        [HttpDelete("{fileId:guid}")]
        public async Task<IActionResult> Delete(Guid fileId)
        {
            try
            {
                var file = await _uploadService.GetFileByIdAsync(fileId);
                if (file == null)
                {
                    _logger.Warning("Delete failed. File not found: {FileId}", fileId);
                    return NotFound(new { success = false, message = "File not found." });
                }

                if (System.IO.File.Exists(file.Path))
                    System.IO.File.Delete(file.Path);

                var success = await _uploadService.DeleteFileAsync(fileId);
                if (!success)
                {
                    _logger.Error("Failed to delete metadata for file: {FileId}", fileId);
                    return StatusCode(500, new { success = false, message = "Failed to delete file metadata." });
                }

                _logger.Information("File deleted: {FileId}", fileId);
                return Ok(new { success = true, message = "File deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "File deletion failed for FileId: {FileId}", fileId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "File deletion failed.",
                    error = ex.Message
                });
            }
        }
    }
}
