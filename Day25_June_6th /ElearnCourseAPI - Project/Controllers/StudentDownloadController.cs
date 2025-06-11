using ElearnAPI.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.RateLimiting;

namespace ElearnAPI.Controllers
{
    [ApiController]
    [EnableRateLimiting("StudentPolicy")]
    [Route("api/v1/student/files")]
    [Authorize(Roles = "Student")]
    public class StudentDownloadController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IUploadService _uploadService;
        private readonly string _uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        public StudentDownloadController(IEnrollmentService enrollmentService, IUploadService uploadService)
        {
            _enrollmentService = enrollmentService;
            _uploadService = uploadService;
        }

        [HttpGet("{courseId}")]
        [HttpGet]
        public async Task<IActionResult> DownloadFiles(Guid courseId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var studentId))
                return Unauthorized(new { success = false, message = "Invalid token." });

            var isEnrolled = await _enrollmentService.IsStudentEnrolledInCourseAsync(studentId, courseId);
            if (!isEnrolled)
                return Forbid("You are not enrolled in this course.");

            var files = await _uploadService.GetFilesByCourseIdAsync(courseId);
            if (files == null || !files.Any())
                return NotFound(new { success = false, message = "No files found for this course." });

            var fileMetas = files
                .Where(f => System.IO.File.Exists(f.Path))
                .Select(f => new
                {
                    f.Id,
                    f.FileName,
                    DownloadUrl = Url.Action("DownloadFile", new { fileId = f.Id }) // link to download each file
                });

            return Ok(new { success = true, data = fileMetas });
        }

        [HttpGet("download/{fileId}")]
        [HttpGet("download")]

        public async Task<IActionResult> DownloadFile(Guid fileId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var studentId))
                return Unauthorized(new { success = false, message = "Invalid token." });

            var file = await _uploadService.GetFileByIdAsync(fileId);
            if (file == null)
                return NotFound(new { success = false, message = "File not found." });

            var isEnrolled = await _enrollmentService.IsStudentEnrolledInCourseAsync(studentId, file.CourseId);
            if (!isEnrolled)
                return Forbid("You are not enrolled in this course.");

            if (!System.IO.File.Exists(file.Path))
                return NotFound(new { success = false, message = "Physical file not found." });

            var fileBytes = await System.IO.File.ReadAllBytesAsync(file.Path);
            var fileName = Path.GetFileName(file.Path);

            return File(fileBytes, "application/octet-stream", fileName);
        }
    }
}
