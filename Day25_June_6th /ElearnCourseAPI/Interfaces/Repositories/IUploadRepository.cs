using ElearnAPI.Models;
using System;
using System.Threading.Tasks;

namespace ElearnAPI.Interfaces.Repositories
{
    public interface IUploadRepository
    {
        Task<UploadedFile?> GetByIdAsync(Guid id);
        Task AddAsync(UploadedFile file);
        Task DeleteAsync(UploadedFile file);
        Task<List<UploadedFile>> GetFilesByCourseIdAsync(Guid courseId);

        Task SaveChangesAsync();

       Task UpdateAsync(UploadedFile file);


    }
}
