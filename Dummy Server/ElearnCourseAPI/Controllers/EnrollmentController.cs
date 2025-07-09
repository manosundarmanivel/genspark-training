using ElearnAPI.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;  
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ElearnAPI.SignalR;
using Microsoft.AspNetCore.SignalR;


namespace ElearnAPI.Controllers
{
    [ApiController]
    [Route("api/v1/enrollments")]
    // [Authorize(Roles = "Student")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly Serilog.ILogger _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public EnrollmentController(IEnrollmentService enrollmentService, IHubContext<NotificationHub> hubContext)
        {
            _enrollmentService = enrollmentService;
            _logger = Log.ForContext<EnrollmentController>();
            _hubContext = hubContext;
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

            var notification = await _enrollmentService.EnrollStudentAsync(userId.Value, courseId);
            if (notification == null)
            {
                _logger.Information("Enrollment failed. User {UserId} may already be enrolled or course {CourseId} is invalid.", userId, courseId);
                return BadRequest(new { success = false, message = "Already enrolled or invalid course." });
            }

            _logger.Information("User {UserId} successfully enrolled in course {CourseId}.", userId, courseId);

            await _hubContext.Clients
                .Group($"instructor-{notification.InstructorId}")
                .SendAsync("ReceiveEnrollmentNotification", notification.StudentName, notification.CourseTitle);

            return Ok(new { success = true, message = "Enrolled successfully." });
        }


        [HttpDelete("{userId}/{courseId}")]
        public async Task<IActionResult> Unenroll(Guid userId, Guid courseId)
        {
            var result = await _enrollmentService.UnenrollStudentAsync(userId, courseId);
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

        [HttpGet("dashboard/{studentId}")]
        public async Task<IActionResult> GetEnrolledCourses(Guid studentId)
        {
            _logger.Information("Fetching enrolled courses for student {StudentId}", studentId);

            var courses = await _enrollmentService.GetEnrolledCourseDetailsAsync(studentId);

            return Ok(new { success = true, data = courses });
        }
        
        [HttpGet("analytics/daily-enrollments")]
[Authorize(Roles = "Admin,Instructor")] 
public async Task<IActionResult> GetDailyEnrollmentStats([FromQuery] int days = 7)
{
    var stats = await _enrollmentService.GetDailyEnrollmentStatsAsync(days);
    _logger.Information("Fetched daily enrollment stats for past {Days} days.", days);
    return Ok(new { success = true, data = stats });
}


    }
}
