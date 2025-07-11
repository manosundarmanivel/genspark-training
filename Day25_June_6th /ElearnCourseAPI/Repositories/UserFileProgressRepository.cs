using ElearnAPI.Data;
using ElearnAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElearnAPI.Interfaces.Repositories;

namespace ElearnAPI.Repositories
{
    public class UserFileProgressRepository : IUserFileProgressRepository
    {
        private readonly ElearnDbContext _context;

        public UserFileProgressRepository(ElearnDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserFileProgress progress)
        {
            await _context.UserFileProgresses.AddAsync(progress);
            await _context.SaveChangesAsync();
        }

        public async Task<UserFileProgress?> GetByUserAndFileAsync(Guid userId, Guid fileId)
        {
            return await _context.UserFileProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.UploadedFileId == fileId);

        }

        public async Task<IEnumerable<UserFileProgress>> GetByUserAsync(Guid userId)
        {
            return await _context.UserFileProgresses
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

       public async Task UpdateAsync(UserFileProgress progress)
{
    Console.WriteLine($"Updating Progress ID: {progress.Id}, IsCompleted: {progress.IsCompleted}, CompletedAt: {progress.CompletedAt}");

    _context.UserFileProgresses.Update(progress);
    int changes = await _context.SaveChangesAsync();

    Console.WriteLine($"EF SaveChanges affected rows: {changes}");
}

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
