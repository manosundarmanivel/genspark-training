using AutoMapper;
using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Repositories;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElearnAPI.Services
{
    public class CourseService : ICourseService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;

        public CourseService(ICourseRepository courseRepository, IEnrollmentRepository enrollmentRepository, IMapper mapper)
        {
            _courseRepository = courseRepository;
            _enrollmentRepository = enrollmentRepository;
            _mapper = mapper;
        }

        public async Task<CourseDto> CreateAsync(CreateCourseDto courseDto, Guid instructorId, string? thumbnailPath)
        {
            var course = _mapper.Map<Course>(courseDto);
            course.InstructorId = instructorId;

            if (!string.IsNullOrEmpty(thumbnailPath))
            {
                course.ThumbnailUrl = thumbnailPath;
            }

            await _courseRepository.AddAsync(course);

            return _mapper.Map<CourseDto>(course);
        }



        public async Task<bool> DeleteAsync(Guid id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null) return false;

            await _courseRepository.DeleteAsync(course);
            return true;
        }

        public async Task<IEnumerable<CourseDto>> GetAllAsync(int page, int pageSize)
        {
            var courses = await _courseRepository.GetAllAsync(page, pageSize);
            return _mapper.Map<IEnumerable<CourseDto>>(courses);
        }

         public async Task<IEnumerable<CourseDto>> GetAllAsyncAdmin(int page, int pageSize)
        {
            var courses = await _courseRepository.GetAllAsyncAdmin(page, pageSize);
            return _mapper.Map<IEnumerable<CourseDto>>(courses);
        }

        public async Task<Course?> GetByIdAsync(Guid id)
        {
            return await _courseRepository.GetByIdWithDetailsAsync(id);
        }



     public async Task<IEnumerable<CourseDto>> GetByInstructorIdAsync(Guid instructorId, int page, int pageSize)
{
    var courses = await _courseRepository.GetByInstructorIdAsync(instructorId, page, pageSize);
    var courseDtos = _mapper.Map<List<CourseDto>>(courses);

    foreach (var courseDto in courseDtos)
    {
        var enrollments = await _enrollmentRepository.GetEnrollmentsByCourseIdAsync(courseDto.Id);
        courseDto.EnrolledStudents = enrollments.Select(e => _mapper.Map<UserDtoResponse>(e.Student)).ToList();
    }

    return courseDtos;
}




        public async Task<bool> UpdateAsync(Guid id, CourseDto courseDto)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null) return false;

            course.Title = courseDto.Title;
            course.Description = courseDto.Description;
            await _courseRepository.UpdateAsync(course);
            return true;
        }

        public async Task<IEnumerable<CourseDto>> SearchByNameAsync(string query)
        {
            var courses = await _courseRepository.SearchByNameAsync(query);
            return _mapper.Map<IEnumerable<CourseDto>>(courses);
        }


        public async Task<List<Guid>> GetCourseIdsByStudentAsync(Guid studentId)
        {
            var enrollments = await _enrollmentRepository.GetEnrollmentsByStudentIdAsync(studentId);
            return enrollments.Select(e => e.CourseId).Distinct().ToList();
        }

        public async Task<Guid?> GetInstructorIdByCourseAsync(Guid courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            return course?.InstructorId;
        }


        public async Task<bool> SetActiveStatusAsync(Guid id, bool isActive)
{
    var course = await _courseRepository.GetByIdAsyncAdmin(id);
    if (course == null) return false;

    course.IsDeleted = isActive;
    course.UpdatedAt = DateTime.UtcNow;

    await _courseRepository.UpdateAsync(course);
    return true;
}



    }
}
