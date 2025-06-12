using ElearnAPI.Controllers;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElearnAPI.Tests.Controllers
{
    [TestFixture]
    public class StudentDownloadControllerTests
    {
        private Mock<IEnrollmentService> _mockEnrollmentService;
        private Mock<IUploadService> _mockUploadService;
        private StudentDownloadController _controller;
        private Guid _studentId;
        private Guid _courseId;

        [SetUp]
        public void SetUp()
        {
            _mockEnrollmentService = new Mock<IEnrollmentService>();
            _mockUploadService = new Mock<IUploadService>();

            _studentId = Guid.NewGuid();
            _courseId = Guid.NewGuid();

            _controller = new StudentDownloadController(_mockEnrollmentService.Object, _mockUploadService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _studentId.ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task DownloadFiles_StudentNotEnrolled_ReturnsForbid()
        {
            _mockEnrollmentService.Setup(s => s.IsStudentEnrolledInCourseAsync(_studentId, _courseId))
                .ReturnsAsync(false);

            var result = await _controller.DownloadFiles(_courseId);

            Assert.IsInstanceOf<ForbidResult>(result);
        }

        [Test]
        public async Task DownloadFiles_NoFiles_ReturnsNotFound()
        {
            _mockEnrollmentService.Setup(s => s.IsStudentEnrolledInCourseAsync(_studentId, _courseId))
                .ReturnsAsync(true);

            _mockUploadService.Setup(s => s.GetFilesByCourseIdAsync(_courseId))
                .ReturnsAsync(new List<UploadedFile>());

            var result = await _controller.DownloadFiles(_courseId) as NotFoundObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public async Task DownloadFile_FileNotFound_ReturnsNotFound()
        {
            var fileId = Guid.NewGuid();

            _mockUploadService.Setup(s => s.GetFileByIdAsync(fileId))
                .ReturnsAsync((UploadedFile)null);

            var result = await _controller.DownloadFile(fileId) as NotFoundObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public async Task DownloadFile_StudentNotEnrolled_ReturnsForbid()
        {
            var file = new UploadedFile
            {
                Id = Guid.NewGuid(),
                FileName = "sample.pdf",
                Path = "nonexistent-path/sample.pdf",
                CourseId = _courseId
            };

            _mockUploadService.Setup(s => s.GetFileByIdAsync(file.Id)).ReturnsAsync(file);
            _mockEnrollmentService.Setup(s => s.IsStudentEnrolledInCourseAsync(_studentId, _courseId))
                .ReturnsAsync(false);

            var result = await _controller.DownloadFile(file.Id);

            Assert.IsInstanceOf<ForbidResult>(result);
        }

        [Test]
        public async Task DownloadFile_FileMissingOnDisk_ReturnsNotFound()
        {
            var file = new UploadedFile
            {
                Id = Guid.NewGuid(),
                FileName = "missing.pdf",
                Path = "missing-file-path.pdf",
                CourseId = _courseId
            };

            _mockUploadService.Setup(s => s.GetFileByIdAsync(file.Id)).ReturnsAsync(file);
            _mockEnrollmentService.Setup(s => s.IsStudentEnrolledInCourseAsync(_studentId, _courseId))
                .ReturnsAsync(true);

            var result = await _controller.DownloadFile(file.Id) as NotFoundObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }
    }
}
