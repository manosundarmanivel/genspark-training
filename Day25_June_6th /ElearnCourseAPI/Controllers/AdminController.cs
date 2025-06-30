using ElearnAPI.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Threading.Tasks;
using ElearnAPI.Data;
using ElearnAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace ElearnAPI.Controllers
{
    [ApiController]
    [Route("api/v1/admin")]
    // [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICourseService _courseService;
        private readonly Serilog.ILogger _logger;

        private readonly ElearnDbContext _context;

        public AdminController(IUserService userService, ICourseService courseService, ElearnDbContext context)
        {
            _userService = userService;
            _courseService = courseService;
            _logger = Log.ForContext<AdminController>();
            _context = context;
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAllData()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Enrollments)
                .Include(u => u.FileProgresses)
                .ToListAsync();

            var roles = await _context.Roles.ToListAsync();

            var courses = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.UploadedFiles)
                .Include(c => c.Enrollments)
                .ToListAsync();

            var files = await _context.UploadedFiles
                .Include(f => f.Course)
                .Include(f => f.FileProgresses)
                .ToListAsync();

            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();

            var fileProgresses = await _context.UserFileProgresses
                .Include(p => p.User)
                .Include(p => p.UploadedFile)
                .ToListAsync();

            return Ok(new
            {
                Users = users,
                Roles = roles,
                Courses = courses,
                Files = files,
                Enrollments = enrollments,
                FileProgresses = fileProgresses
            });
        }

        // -------- USER MANAGEMENT --------

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers(int page = 1, int pageSize = 10)
        {
            var users = await _userService.GetAllAsyncAdmin(page, pageSize);
            _logger.Information("Admin fetched all users");
            return Ok(new { success = true, data = users });
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                _logger.Warning("Admin tried to fetch non-existent user {UserId}", id);
                return NotFound(new { success = false, message = "User not found." });
            }

            _logger.Information("Admin fetched user {UserId}", id);
            return Ok(new { success = true, data = user });
        }

        [HttpPost("users/{id}/enable")]
        public async Task<IActionResult> EnableUser(Guid id)
        {
            var success = await _userService.SetActiveStatusAsync(id, false);
            if (!success)
            {
                _logger.Warning("Admin failed to enable user {UserId}", id);
                return NotFound(new { success = false, message = "User not found." });
            }

            _logger.Information("Admin enabled user {UserId}", id);
            return Ok(new { success = true, message = "User enabled." });
        }

        [HttpPost("users/{id}/disable")]
        public async Task<IActionResult> DisableUser(Guid id)
        {
            var success = await _userService.SetActiveStatusAsync(id, true);
            if (!success)
            {
                _logger.Warning("Admin failed to disable user {UserId}", id);
                return NotFound(new { success = false, message = "User not found." });
            }

            _logger.Information("Admin disabled user {UserId}", id);
            return Ok(new { success = true, message = "User disabled." });
        }

        // -------- COURSE MANAGEMENT --------

        [HttpGet("courses")]
        public async Task<IActionResult> GetAllCourses(int page = 1, int pageSize = 10)
        {
            var courses = await _courseService.GetAllAsyncAdmin(page, pageSize);
            _logger.Information("Admin fetched all courses: Page={Page}, PageSize={PageSize}", page, pageSize);
            return Ok(new { success = true, data = courses });
        }

        [HttpPost("courses/{id}/enable")]
        public async Task<IActionResult> EnableCourse(Guid id)
        {
            var success = await _courseService.SetActiveStatusAsync(id, false);
            if (!success)
            {
                _logger.Warning("Admin failed to enable course {CourseId}", id);
                return NotFound(new { success = false, message = "Course not found." });
            }

            _logger.Information("Admin enabled course {CourseId}", id);
            return Ok(new { success = true, message = "Course enabled." });
        }

        [HttpPost("courses/{id}/disable")]
        public async Task<IActionResult> DisableCourse(Guid id)
        {
            var success = await _courseService.SetActiveStatusAsync(id, true);
            if (!success)
            {
                _logger.Warning("Admin failed to disable course {CourseId}", id);
                return NotFound(new { success = false, message = "Course not found." });
            }

            _logger.Information("Admin disabled course {CourseId}", id);
            return Ok(new { success = true, message = "Course disabled." });
        }
    }
}
