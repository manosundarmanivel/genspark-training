using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElearnAPI.Models;
using ElearnAPI.DTOs;

namespace ElearnAPI.Interfaces.Services
{
    public interface IEnrollmentService
    {
        Task<EnrollmentNotificationDto?> EnrollStudentAsync(Guid userId, Guid courseId);
        Task<bool> UnenrollStudentAsync(Guid userId, Guid courseId);
        Task<IEnumerable<Course>> GetStudentCoursesAsync(Guid userId);
        Task<bool> IsStudentEnrolledInCourseAsync(Guid userId, Guid courseId);
        Task<IEnumerable<UserDtoResponse>> GetStudentsEnrolledInCourseAsync(Guid courseId);

        Task<IEnumerable<Course>> GetEnrolledCourseDetailsAsync(Guid studentId);






    }
}
