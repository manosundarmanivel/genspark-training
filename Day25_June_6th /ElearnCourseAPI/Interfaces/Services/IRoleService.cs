
using ElearnAPI.Models;
using System.Threading.Tasks;

namespace ElearnAPI.Interfaces.Services
{
    public interface IRoleService
    {
        Task<Role?> GetByNameAsync(string name);
    }
}
