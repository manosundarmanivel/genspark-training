using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElearnAPI.Interfaces.Repositories;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.Models;
using ElearnAPI.DTOs;
using AutoMapper;


namespace ElearnAPI.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly IMapper _mapper;

        public EnrollmentService(IEnrollmentRepository enrollmentRepo, IMapper mapper)
        {
            _enrollmentRepo = enrollmentRepo;
            _mapper = mapper;
        }


        public async Task<EnrollmentNotificationDto?> EnrollStudentAsync(Guid userId, Guid courseId)
        {

            var alreadyEnrolled = await _enrollmentRepo.IsEnrolledAsync(userId, courseId);
            if (alreadyEnrolled) return null;


            var newEnrollment = new Enrollment
            {
                UserId = userId,
                CourseId = courseId
            };

            await _enrollmentRepo.AddEnrollmentAsync(newEnrollment);


            var enrollmentDetails = await _enrollmentRepo.GetEnrollmentWithDetailsAsync(userId, courseId);
            if (enrollmentDetails == null) return null;

            return new EnrollmentNotificationDto
            {
                StudentName = enrollmentDetails.Student.FullName ?? enrollmentDetails.Student.Username,
                CourseTitle = enrollmentDetails.Course.Title,
                InstructorId = enrollmentDetails.Course.InstructorId
            };
        }




        public async Task<bool> UnenrollStudentAsync(Guid userId, Guid courseId)
        {
            var enrollment = await _enrollmentRepo.GetEnrollmentAsync(userId, courseId);
            if (enrollment == null) return false;

            await _enrollmentRepo.RemoveEnrollmentAsync(enrollment);
            return true;
        }

        public async Task<IEnumerable<Course>> GetStudentCoursesAsync(Guid userId)
        {
            return await _enrollmentRepo.GetEnrolledCoursesAsync(userId);
        }

        public async Task<bool> IsStudentEnrolledInCourseAsync(Guid userId, Guid courseId)
        {
            return await _enrollmentRepo.IsEnrolledAsync(userId, courseId);
        }

        public async Task<IEnumerable<UserDtoResponse>> GetStudentsEnrolledInCourseAsync(Guid courseId)
        {
            var enrollments = await _enrollmentRepo.GetEnrollmentsByCourseIdAsync(courseId);
            return enrollments.Select(e => _mapper.Map<UserDtoResponse>(e.Student));
        }




        public async Task<IEnumerable<Course>> GetEnrolledCourseDetailsAsync(Guid studentId)
        {
            return await _enrollmentRepo.GetEnrolledCoursesAsync(studentId);
        }
        
        public async Task<List<EnrollmentStatsDto>> GetDailyEnrollmentStatsAsync(int pastDays)
{
    return await _enrollmentRepo.GetDailyEnrollmentStatsAsync(pastDays);
}

    




    }
}
