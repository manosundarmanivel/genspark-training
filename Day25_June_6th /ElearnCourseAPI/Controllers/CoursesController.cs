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

        private readonly IUserFileProgressService _userFileProgressService;

        public CoursesController(
            ICourseService courseService,
            IEnrollmentService enrollmentService,
            IUserFileProgressService userFileProgressService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _userFileProgressService = userFileProgressService;
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

        // [Authorize(Roles = "Instructor")]
        // [EnableRateLimiting("InstructorPolicy")]
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


        // [HttpGet("{id}")]
        // public async Task<IActionResult> GetById(Guid id)
        // {
        //     _logger.Information("Fetching course with ID: {CourseId}", id);
        //     var course = await _courseService.GetByIdAsync(id);

        //     if (course == null)
        //     {
        //         _logger.Warning("Course with ID {CourseId} not found.", id);
        //         return NotFound(new { success = false, message = "Course not found." });
        //     }

        //     return Ok(new { success = true, data = course });
        // }


        // [Authorize(Roles = "Instructor")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
                return NotFound(new { success = false, message = "Course not found." });

            var orderedFiles = course.UploadedFiles.OrderBy(f => f.UploadedAt).ToList();
            var firstFile = orderedFiles.FirstOrDefault();
            bool isEnrolled = false;
            bool isCompleted = false;
            List<Guid> completedFileIds = new();
            Guid userId = Guid.Empty;

            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(userIdClaim, out userId))
                {
                    isEnrolled = course.Enrollments.Any(e => e.UserId == userId);

                    if (isEnrolled)
                    {
                        completedFileIds = await _userFileProgressService.GetCompletedFileIdsAsync(userId, course.Id);
                        isCompleted = orderedFiles.All(f => completedFileIds.Contains(f.Id));
                    }
                }
            }

            var visibleFiles = isEnrolled ? orderedFiles : orderedFiles.Take(1).ToList();
            var thumbnailUrl = string.IsNullOrEmpty(course.ThumbnailUrl)
                ? null
                : Url.Action(nameof(GetThumbnail), "Courses", new { id = course.Id }, Request.Scheme);

            return Ok(new
            {
                success = true,
                data = new
                {
                    course.Id,
                    course.Title,
                    course.Description,
                    course.CreatedAt,
                    course.Level,
                    course.Language,
                    course.Domain,
                    course.Tags,
                    ThumbnailUrl = thumbnailUrl,
                    InstructorName = course.Instructor?.FullName,
                    IsEnrolled = isEnrolled,
                    IsCompleted = isCompleted,

                    FirstUploadedFile = firstFile == null ? null : new
                    {
                        firstFile.Id,
                        firstFile.FileName,
                        firstFile.Topic,
                        firstFile.Description,
                        firstFile.Path,
                        firstFile.UploadedAt,
                        IsCompleted = completedFileIds.Contains(firstFile.Id)
                    },

                    UploadedFiles = visibleFiles.Select(f => new
                    {
                        f.Id,
                        f.FileName,
                        f.Topic,
                        f.Description,
                        f.Path,
                        f.UploadedAt,
                        IsCompleted = completedFileIds.Contains(f.Id)
                    })
                }
            });
        }



        [HttpGet("instructor/{id}")]
        public async Task<IActionResult> GetByIdEdit(Guid id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
                return NotFound(new { success = false, message = "Course not found." });

            var orderedFiles = course.UploadedFiles.OrderBy(f => f.UploadedAt).ToList();
            var firstFile = orderedFiles.FirstOrDefault();
            bool isEnrolled = false;
            bool isCompleted = false;
            List<Guid> completedFileIds = new();
            Guid userId = Guid.Empty;

            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(userIdClaim, out userId))
                {
                    isEnrolled = course.Enrollments.Any(e => e.UserId == userId);

                    if (isEnrolled)
                    {
                        completedFileIds = await _userFileProgressService.GetCompletedFileIdsAsync(userId, course.Id);
                        isCompleted = orderedFiles.All(f => completedFileIds.Contains(f.Id));
                    }
                }
            }

           var visibleFiles = orderedFiles;
            var thumbnailUrl = string.IsNullOrEmpty(course.ThumbnailUrl)
                ? null
                : Url.Action(nameof(GetThumbnail), "Courses", new { id = course.Id }, Request.Scheme);

            return Ok(new
            {
                success = true,
                data = new
                {
                    course.Id,
                    course.Title,
                    course.Description,
                    course.CreatedAt,
                    course.Level,
                    course.Language,
                    course.Domain,
                    course.Tags,
                    ThumbnailUrl = thumbnailUrl,
                    InstructorName = course.Instructor?.FullName,
                    IsEnrolled = isEnrolled,
                    IsCompleted = isCompleted,

                    FirstUploadedFile = firstFile == null ? null : new
                    {
                        firstFile.Id,
                        firstFile.FileName,
                        firstFile.Topic,
                        firstFile.Description,
                        firstFile.Path,
                        firstFile.UploadedAt,
                        IsCompleted = completedFileIds.Contains(firstFile.Id)
                    },

                    UploadedFiles = visibleFiles.Select(f => new
                    {
                        f.Id,
                        f.FileName,
                        f.Topic,
                        f.Description,
                        f.Path,
                        f.UploadedAt,
                        IsCompleted = completedFileIds.Contains(f.Id)
                    })
                }
            });
        }

        [AllowAnonymous]
        [HttpGet("stream/{fileName}")]
        public IActionResult StreamVideo(string fileName)
        {
            var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            var filePath = Path.Combine(uploadsRoot, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { success = false, message = "File not found." });

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, "video/mp4", enableRangeProcessing: true);
        }






        [Authorize(Roles = "Instructor")]
        [HttpGet("{id}/thumbnail")]
        public async Task<IActionResult> GetThumbnail(Guid id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null || string.IsNullOrEmpty(course.ThumbnailUrl))
                return NotFound(new { success = false, message = "Thumbnail not found." });

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), course.ThumbnailUrl);
            if (!System.IO.File.Exists(filePath))
                return NotFound(new { success = false, message = "Thumbnail file missing on server." });

            var contentType = "image/jpeg"; // You could use `Path.GetExtension(filePath)` to determine
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, contentType);
        }






        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateCourseDto courseDto)
        {
            if (!TryGetUserIdFromToken(out var instructorId))
            {
                _logger.Warning("Invalid token while creating course.");
                return Unauthorized(new { success = false, message = "Invalid token." });
            }

            try
            {

                string? savedThumbnailPath = null;
                if (courseDto.Thumbnail != null && courseDto.Thumbnail.Length > 0)
                {
                    var uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "UploadedThumbnails");
                    if (!Directory.Exists(uploadRoot))
                        Directory.CreateDirectory(uploadRoot);

                    var sanitizedFileName = Path.GetFileName(courseDto.Thumbnail.FileName);
                    var uniqueFileName = $"{Guid.NewGuid()}_{sanitizedFileName}";
                    var filePath = Path.Combine(uploadRoot, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await courseDto.Thumbnail.CopyToAsync(stream);
                    }

                    savedThumbnailPath = Path.Combine("UploadedThumbnails", uniqueFileName); // relative path
                }

                var createdCourse = await _courseService.CreateAsync(courseDto, instructorId, savedThumbnailPath);

                _logger.Information("Course created by instructor {InstructorId}: {CourseId}", instructorId, createdCourse.Id);

                return CreatedAtAction(nameof(GetById), new { id = createdCourse.Id }, new { success = true, data = createdCourse });
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

        // [Authorize(Roles = "Instructor")]
      
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

    // if (course.InstructorId != instructorId)
    // {
    //     _logger.Warning("Instructor {InstructorId} tried accessing students of course {CourseId} they do not own.", instructorId, courseId);
    //     return StatusCode(StatusCodes.Status403Forbidden, new { success = false, message = "You do not own this course." });
    // }

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
