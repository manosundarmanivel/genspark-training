using ChienVHShopOnline.Dtos;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;

namespace ChienVHShopOnline.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;

        public OrderService(IOrderRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _repo.GetAllAsync();
            return orders.Select(o => new OrderDto
            {
                OrderID = o.OrderID,
                OrderName = o.OrderName,
                OrderDate = o.OrderDate,
                PaymentType = o.PaymentType,
                Status = o.Status,
                CustomerName = o.CustomerName,
                CustomerPhone = o.CustomerPhone,
                CustomerEmail = o.CustomerEmail,
                CustomerAddress = o.CustomerAddress
            });
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            var o = await _repo.GetByIdAsync(id);
            if (o == null) return null;

            return new OrderDto
            {
                OrderID = o.OrderID,
                OrderName = o.OrderName,
                OrderDate = o.OrderDate,
                PaymentType = o.PaymentType,
                Status = o.Status,
                CustomerName = o.CustomerName,
                CustomerPhone = o.CustomerPhone,
                CustomerEmail = o.CustomerEmail,
                CustomerAddress = o.CustomerAddress
            };
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
        {
            var order = new Order
            {
                OrderName = dto.OrderName,
                OrderDate = dto.OrderDate,
                PaymentType = dto.PaymentType,
                Status = dto.Status,
                CustomerName = dto.CustomerName,
                CustomerPhone = dto.CustomerPhone,
                CustomerEmail = dto.CustomerEmail,
                CustomerAddress = dto.CustomerAddress
            };

            await _repo.AddAsync(order);
            await _repo.SaveChangesAsync();

            return await GetByIdAsync(order.OrderID) ?? throw new Exception("Order creation failed");
        }

        public async Task<OrderDto?> UpdateAsync(UpdateOrderDto dto)
        {
            var o = await _repo.GetByIdAsync(dto.OrderID);
            if (o == null) return null;

            o.OrderName = dto.OrderName;
            o.OrderDate = dto.OrderDate;
            o.PaymentType = dto.PaymentType;
            o.Status = dto.Status;
            o.CustomerName = dto.CustomerName;
            o.CustomerPhone = dto.CustomerPhone;
            o.CustomerEmail = dto.CustomerEmail;
            o.CustomerAddress = dto.CustomerAddress;

            _repo.Update(o);
            await _repo.SaveChangesAsync();

            return await GetByIdAsync(o.OrderID);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var o = await _repo.GetByIdAsync(id);
            if (o == null) return false;

            _repo.Delete(o);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
