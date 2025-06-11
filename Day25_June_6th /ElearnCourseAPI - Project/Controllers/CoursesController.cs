using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.RateLimiting;

namespace ElearnAPI.Controllers
{
    [ApiController]
    [Route("api/v1/courses")]
    
    [Authorize]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;

        public CoursesController(ICourseService courseService, IEnrollmentService enrollmentService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var courses = await _courseService.GetAllAsync(page, pageSize);
            return Ok(new { success = true, data = courses });
        }


        [Authorize(Roles = "Instructor")]
        [EnableRateLimiting("InstructorPolicy")]
        [HttpGet("instructor")]
        public async Task<IActionResult> GetCoursesByInstructor(int page = 1, int pageSize = 10)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var instructorId))
                return Unauthorized(new { success = false, message = "Invalid token." });

            var courses = await _courseService.GetByInstructorIdAsync(instructorId, page, pageSize);
            return Ok(new { success = true, data = courses });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var course = await _courseService.GetByIdAsync(id);
            return course == null ? NotFound() : Ok(new { success = true, data = course });
        }

        [Authorize(Roles = "Instructor")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourseDto courseDto)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var instructorId))
                return Unauthorized(new { success = false, message = "Invalid token." });

            var course = await _courseService.CreateAsync(courseDto, instructorId);
            return CreatedAtAction(nameof(GetById), new { id = course.Id }, new { success = true, data = course });
        }


        [Authorize(Roles = "Instructor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CourseDto courseDto)
        {
            var result = await _courseService.UpdateAsync(id, courseDto);
            return result ? Ok(new { success = true }) : NotFound();
        }

        [Authorize(Roles = "Instructor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _courseService.DeleteAsync(id);
            return result ? Ok(new { success = true }) : NotFound();
        }
        

        [Authorize(Roles = "Instructor")]
[HttpGet("students/{courseId}")]
public async Task<IActionResult> GetStudentsEnrolledInCourse(Guid courseId)
{
    var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (!Guid.TryParse(userIdClaim, out var instructorId))
        return Unauthorized(new { success = false, message = "Invalid token." });


    var course = await _courseService.GetByIdAsync(courseId);
    if (course == null)
        return NotFound(new { success = false, message = "Course not found." });

   
    if (course.InstructorId != instructorId)
        return Forbid("You do not own this course.");

    var students = await _enrollmentService.GetStudentsEnrolledInCourseAsync(courseId);
    return Ok(new { success = true, data = students });
}

    }
}
