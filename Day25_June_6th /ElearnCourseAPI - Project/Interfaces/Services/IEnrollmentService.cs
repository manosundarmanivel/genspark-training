using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElearnAPI.Models;

namespace ElearnAPI.Interfaces.Services
{
    public interface IEnrollmentService
    {
        Task<bool> EnrollStudentAsync(Guid userId, Guid courseId);
        Task<bool> UnenrollStudentAsync(Guid userId, Guid courseId);
        Task<IEnumerable<Course>> GetStudentCoursesAsync(Guid userId);
    }
}
