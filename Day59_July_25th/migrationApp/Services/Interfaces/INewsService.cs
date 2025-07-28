using ChienVHShopOnline.Dtos;

namespace ChienVHShopOnline.Services
{
    public interface INewsService
    {
        Task<IEnumerable<NewsDto>> GetAllAsync();
        Task<NewsDto?> GetByIdAsync(int id);
        Task<NewsDto> CreateAsync(CreateNewsDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(int id, CreateNewsDto dto);
    }
}
