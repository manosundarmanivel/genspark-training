using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElearnAPI.Interfaces.Repositories;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.Models;

namespace ElearnAPI.Services
{
    public class UserFileProgressService : IUserFileProgressService
    {
        private readonly IUserFileProgressRepository _repository;

        public UserFileProgressService(IUserFileProgressRepository repository)
        {
            _repository = repository;
        }

        public async Task AddProgressAsync(UserFileProgress progress)
        {
            await _repository.AddAsync(progress);
        }

        public async Task<UserFileProgress?> GetByUserAndFileAsync(Guid userId, Guid fileId)
        {
            return await _repository.GetByUserAndFileAsync(userId, fileId);
        }

        public async Task<IEnumerable<UserFileProgress>> GetByUserAsync(Guid userId)
        {
            return await _repository.GetByUserAsync(userId);
        }

       public async Task MarkAsCompletedAsync(Guid userId, Guid fileId)
{
    var progress = await _repository.GetByUserAndFileAsync(userId, fileId);

    Console.WriteLine($"[SERVICE LOG] Progress: {(progress == null ? "null" : $"IsCompleted={progress.IsCompleted}")}");


if (progress == null)
{
    progress = new UserFileProgress
    {
        UserId = userId,
        UploadedFileId = fileId,
        IsCompleted = true,
        CompletedAt = DateTime.UtcNow
    };
    await _repository.AddAsync(progress);
}

            // if (progress != null && !progress.IsCompleted)
            // {
            //     progress.IsCompleted = true;
            //     progress.CompletedAt = DateTime.UtcNow;

            //     await _repository.UpdateAsync(progress);
            //     await _repository.SaveChangesAsync();
            // }
}


        public async Task<List<Guid>> GetCompletedFileIdsAsync(Guid userId, Guid courseId)
        {
            var allProgress = await _repository.GetByUserAsync(userId);
            return allProgress
                .Where(p => p.IsCompleted && p.UploadedFile.CourseId == courseId)
                .Select(p => p.UploadedFileId)
                .Distinct()
                .ToList();
        }
    }
}
