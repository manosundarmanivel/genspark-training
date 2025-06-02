using System;
using System.Threading.Tasks;
using BankingApp.DTOs;
using BankingApp.Models;

namespace BankingApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<string> RegisterAsync(UserRegisterDto dto);
        Task<string> LoginAsync(UserLoginDto dto);
        Task<User> GetByIdAsync(Guid id);
    }
}
