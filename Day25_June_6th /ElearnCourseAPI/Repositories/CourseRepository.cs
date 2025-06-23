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
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Course course)
        {
            course.IsDeleted = true;
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
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
                 .Include(c => c.UploadedFiles)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }public async Task<Course?> GetByIdWithDetailsAsync(Guid id)
{
    var course = await _context.Courses
        .Include(c => c.Instructor)
        .Include(c => c.UploadedFiles)
        .Include(c => c.Enrollments) 
        .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

    if (course != null && course.UploadedFiles.Any())
    {
        course.UploadedFiles = course.UploadedFiles
            .OrderBy(f => f.UploadedAt)
            .ToList();
    }

    return course;
}





        public async Task<int> CountAsync()
        {
            return await _context.Courses.CountAsync(c => !c.IsDeleted);
        }
        
        public async Task<IEnumerable<Course>> SearchByNameAsync(string query)
{
    return await _context.Courses
        .Where(c => c.Title.ToLower().Contains(query.ToLower()))
       
        .ToListAsync();
}

    }
}
