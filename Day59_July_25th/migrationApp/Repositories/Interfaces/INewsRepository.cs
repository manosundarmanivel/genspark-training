using ChienVHShopOnline.Models;

namespace ChienVHShopOnline.Repositories
{
    public interface INewsRepository
    {
        Task<IEnumerable<News>> GetAllAsync();
        Task<News?> GetByIdAsync(int id);
        Task AddAsync(News news);
        void Update(News news);
        void Delete(News news);
        Task SaveChangesAsync();
    }
}
