using ChienVHShopOnline.Data;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories
{
    public class ModelRepository : IModelRepository
    {
        private readonly AppDbContext _context;

        public ModelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Model>> GetAllAsync()
        {
            return await _context.Models.ToListAsync();
        }

        public async Task<Model?> GetByIdAsync(int id)
        {
            return await _context.Models.FindAsync(id);
        }

        public async Task AddAsync(Model model)
        {
            await _context.Models.AddAsync(model);
        }

        public void Update(Model model)
        {
            _context.Models.Update(model);
        }

        public void Delete(Model model)
        {
            _context.Models.Remove(model);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
