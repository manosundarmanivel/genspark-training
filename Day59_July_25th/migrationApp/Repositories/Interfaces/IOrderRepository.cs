using ChienVHShopOnline.Models;

namespace ChienVHShopOnline.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(int id);
        Task AddAsync(Order order);
        void Update(Order order);
        void Delete(Order order);
        Task SaveChangesAsync();
    }
}
