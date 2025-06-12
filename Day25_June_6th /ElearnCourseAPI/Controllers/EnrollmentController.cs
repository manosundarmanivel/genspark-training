using ElearnAPI.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;  
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElearnAPI.Controllers
{
    [ApiController]
    [Route("api/v1/enrollments")]
    [Authorize(Roles = "Student")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly Serilog.ILogger _logger;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
            _logger = Log.ForContext<EnrollmentController>(); 
        }

        [HttpPost("{courseId}")]
        public async Task<IActionResult> Enroll(Guid courseId)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                _logger.Warning("Enroll attempt failed due to missing or invalid token.");
                return Unauthorized(new { success = false, message = "Invalid user token." });
            }

            var result = await _enrollmentService.EnrollStudentAsync(userId.Value, courseId);
            if (!result)
            {
                _logger.Information("Enrollment failed. User {UserId} may already be enrolled or course {CourseId} is invalid.", userId, courseId);
                return BadRequest(new { success = false, message = "Already enrolled or invalid course." });
            }

            _logger.Information("User {UserId} successfully enrolled in course {CourseId}.", userId, courseId);
            return Ok(new { success = true, message = "Enrolled successfully." });
        }

        [HttpDelete("{courseId}")]
        public async Task<IActionResult> Unenroll(Guid courseId)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                _logger.Warning("Unenroll attempt failed due to invalid token.");
                return Unauthorized(new { success = false, message = "Invalid user token." });
            }

            var result = await _enrollmentService.UnenrollStudentAsync(userId.Value, courseId);
            if (!result)
            {
                _logger.Information("Unenroll failed. Enrollment not found for user {UserId} in course {CourseId}.", userId, courseId);
                return NotFound(new { success = false, message = "Enrollment not found." });
            }

            _logger.Information("User {UserId} successfully unenrolled from course {CourseId}.", userId, courseId);
            return Ok(new { success = true, message = "Unenrolled successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCourses()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                _logger.Warning("GetMyCourses failed due to missing or invalid token.");
                return Unauthorized(new { success = false, message = "Invalid user token." });
            }

            var courses = await _enrollmentService.GetStudentCoursesAsync(userId.Value);
            _logger.Information("User {UserId} retrieved their enrolled courses.", userId);
            return Ok(new { success = true, data = courses });
        }

        private Guid? GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var id) ? id : null;
        }
    }
}
