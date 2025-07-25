using ChienVHShopOnline.Models;

namespace ChienVHShopOnline.Repositories
{
    public interface IModelRepository
    {
        Task<IEnumerable<Model>> GetAllAsync();
        Task<Model?> GetByIdAsync(int id);
        Task AddAsync(Model model);
        void Update(Model model);
        void Delete(Model model);
        Task SaveChangesAsync();
    }
}
