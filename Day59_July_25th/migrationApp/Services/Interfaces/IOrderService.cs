using ChienVHShopOnline.Dtos;

namespace ChienVHShopOnline.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<OrderDto?> GetByIdAsync(int id);
        Task<OrderDto> CreateAsync(CreateOrderDto dto);
        Task<OrderDto?> UpdateAsync(UpdateOrderDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
