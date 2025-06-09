using AutoMapper;
using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Repositories;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.Models;
using System;
using System.Threading.Tasks;

namespace ElearnAPI.Services
{
    public class UploadService : IUploadService
    {
        private readonly IUploadRepository _uploadRepository;
        private readonly IMapper _mapper;

        public UploadService(IUploadRepository uploadRepository, IMapper mapper)
        {
            _uploadRepository = uploadRepository;
            _mapper = mapper;
        }

        public async Task<UploadedFileDto> UploadFileAsync(UploadedFileDto uploadDto)
        {
            var entity = _mapper.Map<UploadedFile>(uploadDto);
            await _uploadRepository.AddAsync(entity);
            return _mapper.Map<UploadedFileDto>(entity);
        }

        public async Task<bool> DeleteFileAsync(Guid id)
        {
            var file = await _uploadRepository.GetByIdAsync(id);
            if (file == null) return false;
            await _uploadRepository.DeleteAsync(file);
            return true;
        }
    }
}
