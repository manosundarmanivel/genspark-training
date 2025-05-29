using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApp.DTOs;
using BankingApp.Models;
using BankingApp.Repositories.Interfaces;
using BankingApp.Services.Interfaces;

namespace BankingApp.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepo;

        public AccountService(IAccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
        }

        public async Task<Account> CreateAccountAsync(Guid userId, CreateAccountDto dto)
        {
            var account = new Account
            {
                UserId = userId,
                AccountNumber = $"ACCT-{Guid.NewGuid().ToString().Substring(0, 8)}",
                Balance = dto.InitialDeposit
            };

            await _accountRepo.AddAsync(account);
            await _accountRepo.SaveChangesAsync();
            return account;
        }

        public async Task<IEnumerable<Account>> GetUserAccountsAsync(Guid userId)
        {
            return await _accountRepo.GetByUserIdAsync(userId);
        }

        public async Task<Account> GetAccountByIdAsync(Guid id)
        {
            return await _accountRepo.GetByIdAsync(id);
        }
    }
}
