using ElearnAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElearnAPI.Models;

namespace ElearnAPI.Interfaces.Services
{
    public interface ICourseService
    {
        Task<Course> GetByIdAsync(Guid id);
        Task<IEnumerable<CourseDto>> GetAllAsync(int page, int pageSize);
        Task<IEnumerable<CourseDto>> GetByInstructorIdAsync(Guid instructorId, int page, int pageSize);
        Task<CourseDto> CreateAsync(CourseDto courseDto, Guid instructorId);

        Task<bool> UpdateAsync(Guid id, CourseDto courseDto);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<CourseDto>> SearchByNameAsync(string query);

    }
}
