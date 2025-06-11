// Controllers/UploadController.cs
using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
        private readonly string _uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        public UploadController(IUploadService uploadService, IHubContext<NotificationHub> hubContext)
        {
            _uploadService = uploadService;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "Instructor")]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadFileDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdClaim, out var uploadedBy))
                    return Unauthorized(new { success = false, message = "Invalid token. Cannot extract user ID." });

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
                return NotFound(new { success = false, message = "File not found" });

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", sanitizedFileName);
        }
    }
}
