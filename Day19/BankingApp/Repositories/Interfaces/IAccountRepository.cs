using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApp.Models;

namespace BankingApp.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account> GetByIdAsync(Guid id);
        Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId);
        Task AddAsync(Account account);
        Task SaveChangesAsync();
    }
}
