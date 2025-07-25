using ChienVHShopOnline.Dtos;

namespace ChienVHShopOnline.Services
{
    public interface IColorService
    {
        Task<IEnumerable<ColorDto>> GetAllAsync();
        Task<ColorDto?> GetByIdAsync(int id);
        Task<ColorDto> CreateAsync(CreateColorDto dto);
        Task<ColorDto?> UpdateAsync(UpdateColorDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
