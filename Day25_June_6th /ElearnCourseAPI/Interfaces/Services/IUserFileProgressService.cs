using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElearnAPI.Models;

namespace ElearnAPI.Interfaces.Services
{
    public interface IUserFileProgressService
    {
        Task AddProgressAsync(UserFileProgress progress);
        Task<UserFileProgress?> GetByUserAndFileAsync(Guid userId, Guid fileId);
        Task<IEnumerable<UserFileProgress>> GetByUserAsync(Guid userId);
        Task MarkAsCompletedAsync(Guid userId, Guid fileId);
        Task<List<Guid>> GetCompletedFileIdsAsync(Guid userId, Guid courseId);
    }
}
