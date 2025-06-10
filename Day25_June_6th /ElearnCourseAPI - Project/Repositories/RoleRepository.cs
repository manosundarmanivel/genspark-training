// Repositories/RoleRepository.cs
using ElearnAPI.Data;
using ElearnAPI.Interfaces.Repositories;
using ElearnAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ElearnAPI.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ElearnDbContext _context;

        public RoleRepository(ElearnDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == name);
        }
    }
}
