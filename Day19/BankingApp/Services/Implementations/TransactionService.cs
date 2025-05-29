using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApp.DTOs;
using BankingApp.Models;
using BankingApp.Repositories.Interfaces;
using BankingApp.Services.Interfaces;

namespace BankingApp.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly ITransactionRepository _transactionRepo;

        public TransactionService(IAccountRepository accountRepo, ITransactionRepository transactionRepo)
        {
            _accountRepo = accountRepo;
            _transactionRepo = transactionRepo;
        }

      

        public async Task<Transaction> DepositAsync(TransactionDto dto)
        {
            var account = await _accountRepo.GetByIdAsync(dto.AccountId);
            account.Balance += dto.Amount;

            var transaction = new Transaction
            {
                AccountId = account.Id,
                Amount = dto.Amount,
                Type = TransactionType.Deposit,
                Description = dto.Description
            };

            await _transactionRepo.AddAsync(transaction);
            await _transactionRepo.SaveChangesAsync();
            await _accountRepo.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> WithdrawAsync(TransactionDto dto)
        {
            var account = await _accountRepo.GetByIdAsync(dto.AccountId);
            if (account.Balance < dto.Amount) throw new Exception("Insufficient funds");

            account.Balance -= dto.Amount;

            var transaction = new Transaction
            {
                AccountId = account.Id,
                Amount = dto.Amount,
                Type = TransactionType.Withdraw,
                Description = dto.Description
            };

            await _transactionRepo.AddAsync(transaction);
            await _transactionRepo.SaveChangesAsync();
            await _accountRepo.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> TransferAsync(CreateTransactionDto dto)
        {
            var fromAccount = await _accountRepo.GetByIdAsync(dto.AccountId);
            var toAccount = await _accountRepo.GetByIdAsync(dto.TargetAccountId!.Value);

            if (fromAccount.Balance < dto.Amount) throw new Exception("Insufficient funds");

            fromAccount.Balance -= dto.Amount;
            toAccount.Balance += dto.Amount;

            var transaction = new Transaction
            {
                AccountId = fromAccount.Id,
                TargetAccountId = toAccount.Id,
                Amount = dto.Amount,
                Type = TransactionType.Transfer,
                Description = dto.Description
            };

            await _transactionRepo.AddAsync(transaction);
            await _transactionRepo.SaveChangesAsync();
            await _accountRepo.SaveChangesAsync();
            return transaction;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionHistoryAsync(Guid accountId)
        {
            return await _transactionRepo.GetByAccountIdAsync(accountId);
        }
    }
}
