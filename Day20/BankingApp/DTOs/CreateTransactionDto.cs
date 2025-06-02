using System;

namespace BankingApp.DTOs
{
    public class CreateTransactionDto
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public Guid? TargetAccountId { get; set; } 
    }
}
