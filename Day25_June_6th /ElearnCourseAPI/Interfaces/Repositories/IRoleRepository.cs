using ElearnAPI.Models;
using System.Threading.Tasks;

namespace ElearnAPI.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string name);
    }
}