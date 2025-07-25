using ChienVHShopOnline.Dtos;

namespace ChienVHShopOnline.Services
{
    public interface IModelService
    {
        Task<IEnumerable<ModelDto>> GetAllAsync();
        Task<ModelDto?> GetByIdAsync(int id);
        Task<ModelDto> CreateAsync(CreateModelDto dto);
        Task<ModelDto?> UpdateAsync(UpdateModelDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
