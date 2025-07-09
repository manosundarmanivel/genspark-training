using ElearnAPI.Models;
using System.Threading.Tasks;

namespace ElearnAPI.Interfaces.Repositories
{

    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetAllAsync();
        Task<Transaction?> GetByIdAsync(Guid id);
    }

}