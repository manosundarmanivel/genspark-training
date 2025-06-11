using ElearnAPI.Data;
using ElearnAPI.Interfaces.Repositories;
using ElearnAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElearnAPI.Repositories
{
    public class UploadRepository : IUploadRepository
    {
        private readonly ElearnDbContext _context;

        public UploadRepository(ElearnDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UploadedFile file)
        {
            await _context.UploadedFiles.AddAsync(file);
             await _context.SaveChangesAsync(); 
        }

        public async Task DeleteAsync(UploadedFile file)
        {
            _context.UploadedFiles.Remove(file);
             await _context.SaveChangesAsync(); 
        }

        public async Task<UploadedFile?> GetByIdAsync(Guid id)
        {
            return await _context.UploadedFiles.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<List<UploadedFile>> GetFilesByCourseIdAsync(Guid courseId)
        {
            return await _context.UploadedFiles
                                 .Where(f => f.CourseId == courseId)
                                 .ToListAsync();
        }
    }
}
