using ElearnAPI.DTOs;
using System;
using System.Threading.Tasks;

namespace ElearnAPI.Interfaces.Services
{
    public interface IUploadService
    {
        Task<UploadedFileDto> UploadFileAsync(UploadedFileDto uploadDto);
        Task<bool> DeleteFileAsync(Guid id);
    }
}
