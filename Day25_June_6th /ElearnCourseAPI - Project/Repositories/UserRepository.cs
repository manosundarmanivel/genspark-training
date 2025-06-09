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
    public class UserRepository : IUserRepository
    {
        private readonly ElearnDbContext _context;

        public UserRepository(ElearnDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task DeleteAsync(User user)
        {
           
            user.IsDeleted = true;
            _context.Users.Update(user);
        }

        public async Task<IEnumerable<User>> GetAllAsync(int page, int pageSize)
        {
            return await _context.Users
                .Where(u => !u.IsDeleted)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Users.CountAsync(u => !u.IsDeleted);
        }
    }
}
