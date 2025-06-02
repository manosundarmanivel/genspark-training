using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApp.Models;

namespace BankingApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByUsernameAsync(string username);
        Task AddAsync(User user);
    }
}
