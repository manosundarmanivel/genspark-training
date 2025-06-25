using AutoMapper;
using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Repositories;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElearnAPI.Services
{
    public class UploadService : IUploadService
    {
        private readonly IUploadRepository _uploadRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IUserFileProgressRepository _progressRepository;
        private readonly IMapper _mapper;

        public UploadService(
            IUploadRepository uploadRepository,
            IEnrollmentRepository enrollmentRepository,
            IUserFileProgressRepository progressRepository,
            IMapper mapper)
        {
            _uploadRepository = uploadRepository;
            _enrollmentRepository = enrollmentRepository;
            _progressRepository = progressRepository;
            _mapper = mapper;
        }

        public async Task<UploadedFileDto> UploadFileAsync(UploadedFileDto uploadDto)
        {
            // 1. Map and save uploaded file
            var uploadedFile = _mapper.Map<UploadedFile>(uploadDto);
            await _uploadRepository.AddAsync(uploadedFile);

            // 2. Fetch enrolled users
            var enrollments = await _enrollmentRepository.GetEnrollmentsByCourseIdAsync(uploadedFile.CourseId);

            // 3. Create progress records for each user
            var progressRecords = enrollments.Select(e => new UserFileProgress
            {
                UserId = e.UserId,
                UploadedFileId = uploadedFile.Id,
                IsCompleted = false
            });

            foreach (var progress in progressRecords)
            {
                await _progressRepository.AddAsync(progress);
            }

            // 4. Save changes
            await _uploadRepository.SaveChangesAsync();
            await _progressRepository.SaveChangesAsync();

            return _mapper.Map<UploadedFileDto>(uploadedFile);
        }

        public async Task<bool> DeleteFileAsync(Guid id)
        {
            var file = await _uploadRepository.GetByIdAsync(id);
            if (file == null) return false;

            await _uploadRepository.DeleteAsync(file);
            await _uploadRepository.SaveChangesAsync();
            return true;
        }

        public async Task<List<UploadedFile>> GetFilesByCourseIdAsync(Guid courseId)
        {
            return await _uploadRepository.GetFilesByCourseIdAsync(courseId);
            
        }

        public async Task<UploadedFile?> GetFileByIdAsync(Guid fileId)
        {
            return await _uploadRepository.GetByIdAsync(fileId);
        }
    }
}
