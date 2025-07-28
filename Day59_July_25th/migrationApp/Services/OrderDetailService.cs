using ChienVHShopOnline.Dtos;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;

namespace ChienVHShopOnline.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _repo;

        public OrderDetailService(IOrderDetailRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<OrderDetailDto>> GetAllAsync()
        {
            var details = await _repo.GetAllAsync();
            return details.Select(d => new OrderDetailDto
            {
                OrderID = d.OrderID,
                ProductID = d.ProductID,
                Price = d.Price,
                Quantity = d.Quantity
            });
        }

        public async Task<OrderDetailDto?> GetByIdAsync(int orderId, int productId)
        {
            var d = await _repo.GetByIdAsync(orderId, productId);
            if (d == null) return null;

            return new OrderDetailDto
            {
                OrderID = d.OrderID,
                ProductID = d.ProductID,
                Price = d.Price,
                Quantity = d.Quantity
            };
        }

        public async Task<OrderDetailDto> CreateAsync(CreateOrderDetailDto dto)
        {
            var orderDetail = new OrderDetail
            {
                OrderID = dto.OrderID,
                ProductID = dto.ProductID,
                Price = dto.Price,
                Quantity = dto.Quantity
            };

            await _repo.AddAsync(orderDetail);
            await _repo.SaveChangesAsync();

            return await GetByIdAsync(dto.OrderID, dto.ProductID)
                   ?? throw new Exception("Creation failed");
        }

        public async Task<bool> DeleteAsync(int orderId, int productId)
        {
            var existing = await _repo.GetByIdAsync(orderId, productId);
            if (existing == null) return false;

            _repo.Delete(existing);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
