using ChienVHShopOnline.Models;

namespace ChienVHShopOnline.Repositories
{
    public interface IOrderDetailRepository
    {
        Task<IEnumerable<OrderDetail>> GetAllAsync();
        Task<OrderDetail?> GetByIdAsync(int orderId, int productId);
        Task AddAsync(OrderDetail orderDetail);
        void Update(OrderDetail orderDetail);
        void Delete(OrderDetail orderDetail);
        Task SaveChangesAsync();
    }
}
