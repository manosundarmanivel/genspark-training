using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankingApp.Data;
using BankingApp.Models;
using BankingApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Repositories.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankingDbContext _context;

        public AccountRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<Account> GetByIdAsync(Guid id)
        {
            return await _context.Accounts.Include(a => a.Transactions).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Accounts.Where(a => a.UserId == userId).ToListAsync();
        }

        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
