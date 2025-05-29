using BankingApp.DTOs;
using BankingApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(TransactionDto dto)
        {
            dto.Description = "Deposit";
            var result = await _transactionService.DepositAsync(dto);
            return Ok(result);
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(TransactionDto dto)
        {
            dto.Description = "Withdraw";
            var result = await _transactionService.WithdrawAsync(dto);
            return Ok(result);
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(CreateTransactionDto dto)
        {
            dto.Description = "Transfer";
            var result = await _transactionService.TransferAsync(dto);
            return Ok(result);
        }

        
    }
}
