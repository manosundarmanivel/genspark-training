using ElearnAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElearnAPI.Interfaces.Services
{
    public interface ICourseService
    {
        Task<CourseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<CourseDto>> GetAllAsync(int page, int pageSize);
        Task<IEnumerable<CourseDto>> GetByInstructorIdAsync(Guid instructorId, int page, int pageSize);
        Task<CourseDto> CreateAsync(CourseDto courseDto);
        Task<bool> UpdateAsync(Guid id, CourseDto courseDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
