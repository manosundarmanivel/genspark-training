using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BankingApp.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Username { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; }

        public string Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
