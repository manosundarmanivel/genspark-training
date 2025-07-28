using ChienVHShopOnline.Data;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly AppDbContext _context;

        public OrderDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDetail>> GetAllAsync()
        {
            return await _context.OrderDetails
                .Include(o => o.Order)
                .Include(p => p.Product)
                .ToListAsync();
        }

        public async Task<OrderDetail?> GetByIdAsync(int orderId, int productId)
        {
            return await _context.OrderDetails
                .Include(o => o.Order)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(x => x.OrderID == orderId && x.ProductID == productId);
        }

        public async Task AddAsync(OrderDetail orderDetail)
        {
            await _context.OrderDetails.AddAsync(orderDetail);
        }

        public void Update(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
        }

        public void Delete(OrderDetail orderDetail)
        {
            _context.OrderDetails.Remove(orderDetail);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
