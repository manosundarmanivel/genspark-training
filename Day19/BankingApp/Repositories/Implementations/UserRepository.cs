using System;
using System.Threading.Tasks;
using BankingApp.Data;
using BankingApp.Models;
using BankingApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly BankingDbContext _context;

        public UserRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _context.Users.Include(u => u.Accounts).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}
