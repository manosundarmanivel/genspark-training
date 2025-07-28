using ChienVHShopOnline.Data;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly AppDbContext _context;

        public NewsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<News>> GetAllAsync()
        {
            return await _context.News.Include(n => n.User).ToListAsync();
        }

        public async Task<News?> GetByIdAsync(int id)
        {
            return await _context.News.Include(n => n.User).FirstOrDefaultAsync(n => n.NewsId == id);
        }

        public async Task AddAsync(News news)
        {
            await _context.News.AddAsync(news);
        }

        public void Update(News news)
        {
            _context.News.Update(news);
        }

        public void Delete(News news)
        {
            _context.News.Remove(news);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
