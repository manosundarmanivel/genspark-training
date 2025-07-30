using ChienVHShopOnline.Models;

namespace ChienVHShopOnline.Repositories
{
    public interface IContactURepository
    {
        Task<IEnumerable<ContactU>> GetAllAsync();
        Task<ContactU?> GetByIdAsync(int id);
        Task<ContactU> CreateAsync(ContactU contact);
        Task<bool> DeleteAsync(int id);
    }
}
