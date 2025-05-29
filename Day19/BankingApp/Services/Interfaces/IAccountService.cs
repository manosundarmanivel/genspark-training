using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApp.DTOs;
using BankingApp.Models;

namespace BankingApp.Services.Interfaces
{
    public interface IAccountService
    {
        Task<Account> CreateAccountAsync(Guid userId, CreateAccountDto dto);
        Task<IEnumerable<Account>> GetUserAccountsAsync(Guid userId);
        Task<Account> GetAccountByIdAsync(Guid id);
    }
}
