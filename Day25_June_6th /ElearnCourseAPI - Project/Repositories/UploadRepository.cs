using ElearnAPI.Data;
using ElearnAPI.Interfaces.Repositories;
using ElearnAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
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
        }

        public async Task DeleteAsync(UploadedFile file)
        {
            _context.UploadedFiles.Remove(file);
        }

        public async Task<UploadedFile?> GetByIdAsync(Guid id)
        {
            return await _context.UploadedFiles.FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}
