using ChienVHShopOnline.Dtos;

namespace ChienVHShopOnline.Services
{
    public interface IContactUService
    {
        Task<IEnumerable<ContactUDto>> GetAllAsync();
        Task<ContactUDto?> GetByIdAsync(int id);
        Task<ContactUDto> CreateAsync(CreateContactUDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
