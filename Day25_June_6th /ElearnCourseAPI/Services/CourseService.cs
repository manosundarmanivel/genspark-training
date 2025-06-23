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
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;

        public CourseService(ICourseRepository courseRepository, IMapper mapper)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
        }

        public async Task<CourseDto> CreateAsync(CourseDto courseDto, Guid instructorId)
        {
            var course = _mapper.Map<Course>(courseDto);
            course.InstructorId = instructorId; // Attach instructor ID
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

 public async Task<Course?> GetByIdAsync(Guid id)
    {
        return await _courseRepository.GetByIdWithDetailsAsync(id);
    }

        

        public async Task<IEnumerable<CourseDto>> GetByInstructorIdAsync(Guid instructorId, int page, int pageSize)
        {
            var courses = await _courseRepository.GetByInstructorIdAsync(instructorId, page, pageSize);
            return _mapper.Map<IEnumerable<CourseDto>>(courses);
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

    }
}
