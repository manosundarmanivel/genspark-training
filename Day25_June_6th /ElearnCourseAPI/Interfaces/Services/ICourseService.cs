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
        Task<IEnumerable<CourseDto>> GetAllAsyncAdmin(int page, int pageSize);
        Task<IEnumerable<CourseDto>> GetByInstructorIdAsync(Guid instructorId, int page, int pageSize);
        Task<CourseDto> CreateAsync(CreateCourseDto courseDto, Guid instructorId, string? thumbnailPath);

        Task<bool> UpdateAsync(Guid id, CourseDto courseDto);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<CourseDto>> SearchByNameAsync(string query);

        Task<List<Guid>> GetCourseIdsByStudentAsync(Guid studentId);
        Task<Guid?> GetInstructorIdByCourseAsync(Guid courseId);

        Task<bool> SetActiveStatusAsync(Guid id, bool isActive);


    }
}
