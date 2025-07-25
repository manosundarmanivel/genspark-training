using ChienVHShopOnline.Dtos;

namespace ChienVHShopOnline.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<ProductDto?> UpdateAsync(UpdateProductDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
