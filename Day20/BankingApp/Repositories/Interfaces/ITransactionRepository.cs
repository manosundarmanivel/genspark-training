using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApp.Models;

namespace BankingApp.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId);
        Task AddAsync(Transaction transaction);
        Task SaveChangesAsync();
    }
}
