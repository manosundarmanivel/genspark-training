using ElearnAPI.DTOs;
using System;
using System.Threading.Tasks;
using ElearnAPI.Models;


namespace ElearnAPI.Interfaces.Services
{
    public interface IUploadService
    {
        Task<UploadedFileDto> UploadFileAsync(UploadedFileDto uploadDto);
        Task<bool> DeleteFileAsync(Guid id);
        Task<List<UploadedFile>> GetFilesByCourseIdAsync(Guid courseId);
        Task<UploadedFile?> GetFileByIdAsync(Guid fileId);

    }
}
