using System;

namespace BankingApp.DTOs
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        
    }
}
