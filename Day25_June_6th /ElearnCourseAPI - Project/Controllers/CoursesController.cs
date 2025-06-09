using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ElearnAPI.Controllers
{
    [ApiController]
    [Route("api/v1/courses")]
    [Authorize]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var courses = await _courseService.GetAllAsync(page, pageSize);
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
            var course = await _courseService.CreateAsync(courseDto);
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
    }
}
