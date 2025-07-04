using ElearnAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElearnAPI.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByIdAsyncAdmin(Guid id);
        Task<User?> GetByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllAsync(int page, int pageSize);
        Task<IEnumerable<User>> GetAllAsyncAdmin(int page, int pageSize);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<int> CountAsync();

        Task<User?> GetByRefreshTokenAsync(string refreshToken);

    }
}
