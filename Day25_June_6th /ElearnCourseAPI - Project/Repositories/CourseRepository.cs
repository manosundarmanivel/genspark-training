using ElearnAPI.Data;
using ElearnAPI.Interfaces.Repositories;
using ElearnAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ElearnAPI.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ElearnDbContext _context;

        public CourseRepository(ElearnDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
        }

        public async Task DeleteAsync(Course course)
        {
            course.IsDeleted = true;
            _context.Courses.Update(course);
        }

        public async Task<IEnumerable<Course>> GetAllAsync(int page, int pageSize)
        {
            return await _context.Courses
                .Where(c => !c.IsDeleted)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Course?> GetByIdAsync(Guid id)
        {
            return await _context.Courses.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<IEnumerable<Course>> GetByInstructorIdAsync(Guid instructorId, int page, int pageSize)
        {
            return await _context.Courses
                .Where(c => c.InstructorId == instructorId && !c.IsDeleted)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Courses.CountAsync(c => !c.IsDeleted);
        }
    }
}
