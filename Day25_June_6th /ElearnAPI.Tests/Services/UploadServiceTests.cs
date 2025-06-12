using AutoMapper;
using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Repositories;
using ElearnAPI.Models;
using ElearnAPI.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElearnAPI.Tests.Services
{
    public class UploadServiceTests
    {
        private Mock<IUploadRepository> _uploadRepositoryMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private UploadService _uploadService = null!;

        [SetUp]
        public void Setup()
        {
            _uploadRepositoryMock = new Mock<IUploadRepository>();
            _mapperMock = new Mock<IMapper>();
            _uploadService = new UploadService(_uploadRepositoryMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task UploadFileAsync_ShouldMapAndSaveAndReturnDto()
        {
            var courseId = Guid.NewGuid();
            var fileDto = new UploadedFileDto
            {
                FileName = "test.pdf",
                Path = "/files/test.pdf",
                CourseId = courseId
            };

            var fileEntity = new UploadedFile
            {
                Id = Guid.NewGuid(),
                FileName = fileDto.FileName,
                Path = fileDto.Path,
                CourseId = fileDto.CourseId
            };

            _mapperMock.Setup(m => m.Map<UploadedFile>(fileDto)).Returns(fileEntity);
            _mapperMock.Setup(m => m.Map<UploadedFileDto>(fileEntity)).Returns(fileDto);

            var result = await _uploadService.UploadFileAsync(fileDto);

            _uploadRepositoryMock.Verify(r => r.AddAsync(fileEntity), Times.Once);
            Assert.That(result.FileName, Is.EqualTo("test.pdf"));
            Assert.That(result.Path, Is.EqualTo("/files/test.pdf"));
        }

        [Test]
        public async Task DeleteFileAsync_FileExists_ReturnsTrue()
        {
            var fileId = Guid.NewGuid();
            var file = new UploadedFile
            {
                Id = fileId,
                FileName = "delete.pdf",
                Path = "/files/delete.pdf",
                CourseId = Guid.NewGuid()
            };

            _uploadRepositoryMock.Setup(r => r.GetByIdAsync(fileId)).ReturnsAsync(file);

            var result = await _uploadService.DeleteFileAsync(fileId);

            _uploadRepositoryMock.Verify(r => r.DeleteAsync(file), Times.Once);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task DeleteFileAsync_FileDoesNotExist_ReturnsFalse()
        {
            var fileId = Guid.NewGuid();

            _uploadRepositoryMock.Setup(r => r.GetByIdAsync(fileId)).ReturnsAsync((UploadedFile?)null);

            var result = await _uploadService.DeleteFileAsync(fileId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetFilesByCourseIdAsync_ShouldReturnFiles()
        {
            var courseId = Guid.NewGuid();
            var files = new List<UploadedFile>
            {
                new UploadedFile
                {
                    Id = Guid.NewGuid(),
                    FileName = "coursefile.pdf",
                    Path = "/files/coursefile.pdf",
                    CourseId = courseId
                }
            };

            _uploadRepositoryMock.Setup(r => r.GetFilesByCourseIdAsync(courseId)).ReturnsAsync(files);

            var result = await _uploadService.GetFilesByCourseIdAsync(courseId);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].FileName, Is.EqualTo("coursefile.pdf"));
        }

        [Test]
        public async Task GetFileByIdAsync_FileExists_ReturnsFile()
        {
            var fileId = Guid.NewGuid();
            var file = new UploadedFile
            {
                Id = fileId,
                FileName = "file.pdf",
                Path = "/files/file.pdf",
                CourseId = Guid.NewGuid()
            };

            _uploadRepositoryMock.Setup(r => r.GetByIdAsync(fileId)).ReturnsAsync(file);

            var result = await _uploadService.GetFileByIdAsync(fileId);

            Assert.That(result?.FileName, Is.EqualTo("file.pdf"));
        }

        [Test]
        public async Task GetFileByIdAsync_FileNotFound_ReturnsNull()
        {
            var fileId = Guid.NewGuid();
            _uploadRepositoryMock.Setup(r => r.GetByIdAsync(fileId)).ReturnsAsync((UploadedFile?)null);

            var result = await _uploadService.GetFileByIdAsync(fileId);

            Assert.IsNull(result);
        }
    }
}
