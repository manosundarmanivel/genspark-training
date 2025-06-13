using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Security.Claims;

namespace ElearnAPI.Controllers
{
    [ApiController]
    [Route("api/v1/courses")]
    [Authorize]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly Serilog.ILogger _logger;

        public CoursesController(ICourseService courseService, IEnrollmentService enrollmentService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _logger = Log.ForContext<CoursesController>();
        }

        private bool TryGetUserIdFromToken(out Guid userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out userId);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            _logger.Information("Fetching all courses. Page: {Page}, PageSize: {PageSize}", page, pageSize);
            var courses = await _courseService.GetAllAsync(page, pageSize);
            return Ok(new { success = true, data = courses });
        }

        [Authorize(Roles = "Instructor")]
        [EnableRateLimiting("InstructorPolicy")]
        [HttpGet("instructor")]
        public async Task<IActionResult> GetCoursesByInstructor(int page = 1, int pageSize = 10)
        {
            if (!TryGetUserIdFromToken(out var instructorId))
            {
                _logger.Warning("Invalid token detected while fetching instructor courses.");
                return Unauthorized(new { success = false, message = "Invalid token." });
            }

            _logger.Information("Fetching courses for instructor {InstructorId}", instructorId);
            var courses = await _courseService.GetByInstructorIdAsync(instructorId, page, pageSize);
            return Ok(new { success = true, data = courses });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.Information("Fetching course with ID: {CourseId}", id);
            var course = await _courseService.GetByIdAsync(id);

            if (course == null)
            {
                _logger.Warning("Course with ID {CourseId} not found.", id);
                return NotFound(new { success = false, message = "Course not found." });
            }

            return Ok(new { success = true, data = course });
        }

        [Authorize(Roles = "Instructor")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourseDto courseDto)
        {
            if (!TryGetUserIdFromToken(out var instructorId))
            {
                _logger.Warning("Invalid token while creating course.");
                return Unauthorized(new { success = false, message = "Invalid token." });
            }

            try
            {
                var course = await _courseService.CreateAsync(courseDto, instructorId);
                _logger.Information("Course created by instructor {InstructorId}: {CourseId}", instructorId, course.Id);

                return CreatedAtAction(nameof(GetById), new { id = course.Id }, new { success = true, data = course });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Course creation failed.");
                return StatusCode(500, new { success = false, message = "Course creation failed.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Instructor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CourseDto courseDto)
        {
            try
            {
                var result = await _courseService.UpdateAsync(id, courseDto);
                if (!result)
                {
                    _logger.Warning("Course update failed for ID {CourseId}", id);
                    return NotFound(new { success = false, message = "Course not found." });
                }

                _logger.Information("Course updated successfully for ID {CourseId}", id);
                return Ok(new { success = true, message = "Course updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Course update failed for ID {CourseId}", id);
                return StatusCode(500, new { success = false, message = "Course update failed.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Instructor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _courseService.DeleteAsync(id);
                if (!result)
                {
                    _logger.Warning("Course deletion failed for ID {CourseId}", id);
                    return NotFound(new { success = false, message = "Course not found." });
                }

                _logger.Information("Course deleted successfully for ID {CourseId}", id);
                return Ok(new { success = true, message = "Course deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Course deletion failed for ID {CourseId}", id);
                return StatusCode(500, new { success = false, message = "Course deletion failed.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Instructor")]
        [HttpGet("students/{courseId}")]
        public async Task<IActionResult> GetStudentsEnrolledInCourse(Guid courseId)
        {
            if (!TryGetUserIdFromToken(out var instructorId))
            {
                _logger.Warning("Invalid token for instructor while accessing student list.");
                return Unauthorized(new { success = false, message = "Invalid token." });
            }

            var course = await _courseService.GetByIdAsync(courseId);
            if (course == null)
            {
                _logger.Warning("Course with ID {CourseId} not found for instructor {InstructorId}", courseId, instructorId);
                return NotFound(new { success = false, message = "Course not found." });
            }

            if (course.InstructorId != instructorId)
            {
                _logger.Warning("Instructor {InstructorId} tried accessing students of course {CourseId} they do not own.", instructorId, courseId);
                return Forbid("You do not own this course.");
            }

            var students = await _enrollmentService.GetStudentsEnrolledInCourseAsync(courseId);
            _logger.Information("Fetched students for course {CourseId} by instructor {InstructorId}", courseId, instructorId);

            return Ok(new { success = true, data = students });
        }

 [Authorize(Roles = "Student")]
        [HttpGet("search")]
public async Task<IActionResult> SearchCourses([FromQuery] string query)
{
    if (string.IsNullOrWhiteSpace(query))
        return BadRequest(new { success = false, message = "Search query is required." });

    _logger.Information("Searching courses with query: {Query}", query);
    var results = await _courseService.SearchByNameAsync(query);

    return Ok(new { success = true, data = results });
}

    }
}
