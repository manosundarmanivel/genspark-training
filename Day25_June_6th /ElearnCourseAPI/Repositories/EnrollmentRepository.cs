using ElearnAPI.Data;
using ElearnAPI.Interfaces.Repositories;
using ElearnAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElearnAPI.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly ElearnDbContext _context;

        public EnrollmentRepository(ElearnDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsEnrolledAsync(Guid userId, Guid courseId)
        {
            return await _context.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
        }

        public async Task AddEnrollmentAsync(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task<Enrollment?> GetEnrollmentAsync(Guid userId, Guid courseId)
        {
            return await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
        }

        public async Task RemoveEnrollmentAsync(Enrollment enrollment)
        {
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Course>> GetEnrolledCoursesAsync(Guid userId)
        {
            return await _context.Enrollments
                .Where(e => e.UserId == userId)
                .Select(e => e.Course)
                .ToListAsync();
        }




        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdAsync(Guid courseId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentIdAsync(Guid studentId)
        {
            return await _context.Enrollments
                .Where(e => e.UserId == studentId)
                .ToListAsync();
        }


        public async Task<Enrollment?> GetEnrollmentWithDetailsAsync(Guid userId, Guid courseId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
        }






    }
}
