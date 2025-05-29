using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingApp.Models
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid AccountId { get; set; }

        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string Description { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

     
        public Guid? TargetAccountId { get; set; }

        [ForeignKey(nameof(TargetAccountId))]
        public Account TargetAccount { get; set; }
    }

    public enum TransactionType
    {
        Deposit,
        Withdraw,
        Transfer
    }
}
