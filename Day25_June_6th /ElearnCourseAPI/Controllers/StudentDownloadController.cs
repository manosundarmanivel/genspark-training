using ElearnAPI.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
        private readonly Serilog.ILogger _logger;
        private readonly string _uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        public StudentDownloadController(IEnrollmentService enrollmentService, IUploadService uploadService)
        {
            _enrollmentService = enrollmentService;
            _uploadService = uploadService;
            _logger = Log.ForContext<StudentDownloadController>();
        }

        private bool TryGetUserIdFromToken(out Guid userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out userId);
        }

        [HttpGet("{courseId}")]
        public async Task<IActionResult> DownloadFiles(Guid courseId)
        {
            _logger.Information("Student requested file list for course {CourseId}", courseId);

            if (!TryGetUserIdFromToken(out var studentId))
            {
                _logger.Warning("Invalid token while accessing files for course {CourseId}", courseId);
                return Unauthorized(new { success = false, message = "Invalid token." });
            }

            var isEnrolled = await _enrollmentService.IsStudentEnrolledInCourseAsync(studentId, courseId);
            if (!isEnrolled)
            {
                _logger.Warning("Student {StudentId} attempted to access course {CourseId} without enrollment", studentId, courseId);
                return Forbid("You are not enrolled in this course.");
            }

            var files = await _uploadService.GetFilesByCourseIdAsync(courseId);
            if (files == null || !files.Any())
            {
                _logger.Information("No files found for course {CourseId}", courseId);
                return NotFound(new { success = false, message = "No files found for this course." });
            }

            var fileMetas = files
                .Where(f => System.IO.File.Exists(f.Path))
                .Select(f => new
                {
                    f.Id,
                    f.FileName,
                    f.Topic,
                    f.Description,
                    DownloadUrl = Url.Action(nameof(DownloadFile), new { fileId = f.Id })
                });

            _logger.Information("Returning {Count} files for student {StudentId} in course {CourseId}", fileMetas.Count(), studentId, courseId);
            return Ok(new { success = true, data = fileMetas });
        }

        [HttpGet("download/{fileId}")]
        public async Task<IActionResult> DownloadFile(Guid fileId)
        {
            _logger.Information("Download request received for file {FileId}", fileId);

            if (!TryGetUserIdFromToken(out var studentId))
            {
                _logger.Warning("Invalid token while downloading file {FileId}", fileId);
                return Unauthorized(new { success = false, message = "Invalid token." });
            }

            try
            {
                var file = await _uploadService.GetFileByIdAsync(fileId);
                if (file == null)
                {
                    _logger.Warning("Requested file {FileId} not found", fileId);
                    return NotFound(new { success = false, message = "File not found." });
                }

                var isEnrolled = await _enrollmentService.IsStudentEnrolledInCourseAsync(studentId, file.CourseId);
                if (!isEnrolled)
                {
                    _logger.Warning("Student {StudentId} not enrolled in course {CourseId}, access denied to file {FileId}", studentId, file.CourseId, fileId);
                    return Forbid("You are not enrolled in this course.");
                }

                if (!System.IO.File.Exists(file.Path))
                {
                    _logger.Error("Physical file missing at path {Path} for file {FileId}", file.Path, fileId);
                    return NotFound(new { success = false, message = "Physical file not found." });
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(file.Path);
                var fileName = Path.GetFileName(file.Path);

                _logger.Information("Serving file {FileId} to student {StudentId}", fileId, studentId);
                return File(fileBytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error while downloading file {FileId}", fileId);
                return StatusCode(500, new { success = false, message = "An error occurred while downloading the file.", error = ex.Message });
            }
        }
    }
}
