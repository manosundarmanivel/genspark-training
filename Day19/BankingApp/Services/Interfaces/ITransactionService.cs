using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApp.DTOs;
using BankingApp.Models;

namespace BankingApp.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> DepositAsync(TransactionDto dto);
        Task<Transaction> WithdrawAsync(TransactionDto dto);
        Task<Transaction> TransferAsync(CreateTransactionDto dto);
        Task<IEnumerable<Transaction>> GetTransactionHistoryAsync(Guid accountId);
    }
}
