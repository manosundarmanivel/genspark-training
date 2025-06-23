using ElearnAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElearnAPI.Interfaces.Repositories
{
    public interface IUserFileProgressRepository
    {
        Task AddAsync(UserFileProgress progress);
        Task<UserFileProgress?> GetByUserAndFileAsync(Guid userId, Guid fileId);
        Task<IEnumerable<UserFileProgress>> GetByUserAsync(Guid userId);
        Task UpdateAsync(UserFileProgress progress);
        Task SaveChangesAsync();
    }
}
