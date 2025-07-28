using ChienVHShopOnline.Dtos;

namespace ChienVHShopOnline.Services
{
    public interface IOrderDetailService
    {
        Task<IEnumerable<OrderDetailDto>> GetAllAsync();
        Task<OrderDetailDto?> GetByIdAsync(int orderId, int productId);
        Task<OrderDetailDto> CreateAsync(CreateOrderDetailDto dto);
        Task<bool> DeleteAsync(int orderId, int productId);
    }
}
