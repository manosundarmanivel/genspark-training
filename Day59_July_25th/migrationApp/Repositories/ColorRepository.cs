using ChienVHShopOnline.Data;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories
{
    public class ColorRepository : IColorRepository
    {
        private readonly AppDbContext _context;

        public ColorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Color>> GetAllAsync()
        {
            return await _context.Colors.ToListAsync();
        }

        public async Task<Color?> GetByIdAsync(int id)
        {
            return await _context.Colors.FindAsync(id);
        }

        public async Task AddAsync(Color color)
        {
            await _context.Colors.AddAsync(color);
        }

        public void Update(Color color)
        {
            _context.Colors.Update(color);
        }

        public void Delete(Color color)
        {
            _context.Colors.Remove(color);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
