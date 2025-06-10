using ElearnAPI.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

       
        [HttpPost("{courseId}")]
        public async Task<IActionResult> Enroll(Guid courseId)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { success = false, message = "Invalid user token." });

            var result = await _enrollmentService.EnrollStudentAsync(userId.Value, courseId);
            if (!result)
                return BadRequest(new { success = false, message = "Already enrolled or invalid course." });

            return Ok(new { success = true, message = "Enrolled successfully." });
        }


        [HttpDelete("{courseId}")]
        public async Task<IActionResult> Unenroll(Guid courseId)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { success = false, message = "Invalid user token." });

            var result = await _enrollmentService.UnenrollStudentAsync(userId.Value, courseId);
            if (!result)
                return NotFound(new { success = false, message = "Enrollment not found." });

            return Ok(new { success = true, message = "Unenrolled successfully." });
        }

       
        [HttpGet]
        public async Task<IActionResult> GetMyCourses()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { success = false, message = "Invalid user token." });

            var courses = await _enrollmentService.GetStudentCoursesAsync(userId.Value);
            return Ok(new { success = true, data = courses });
        }

       
        private Guid? GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var id) ? id : null;
        }
    }
}
