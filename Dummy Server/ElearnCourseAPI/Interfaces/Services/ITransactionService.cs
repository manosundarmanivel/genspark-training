using ElearnAPI.DTOs;
using System;
using System.Threading.Tasks;
using ElearnAPI.Models;


namespace ElearnAPI.Interfaces.Services
{
    public interface ITransactionService
{
    Task<TransactionDto> CreateAsync(CreateTransactionDto dto);
    Task<IEnumerable<TransactionDto>> GetAllAsync();
    Task<TransactionDto?> GetByIdAsync(Guid id);
}

}
    