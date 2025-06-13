using ElearnAPI.Controllers;
using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Services;
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
    public class CoursesControllerTests
    {
        private Mock<ICourseService> _mockCourseService;
        private Mock<IEnrollmentService> _mockEnrollmentService;
        private CoursesController _controller;

        [SetUp]
        public void Setup()
        {
            _mockCourseService = new Mock<ICourseService>();
            _mockEnrollmentService = new Mock<IEnrollmentService>();
            _controller = new CoursesController(_mockCourseService.Object, _mockEnrollmentService.Object);
        }

        private void SetUserWithId(Guid userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task GetAll_ReturnsOkWithCourses()
        {
            _mockCourseService.Setup(s => s.GetAllAsync(1, 10)).ReturnsAsync(new List<CourseDto>());

            var result = await _controller.GetAll();

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetById_ExistingId_ReturnsOk()
        {
            var id = Guid.NewGuid();
            _mockCourseService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(new CourseDto { Id = id });

            var result = await _controller.GetById(id);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetById_NonExistingId_ReturnsNotFound()
        {
            _mockCourseService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((CourseDto?)null);

            var result = await _controller.GetById(Guid.NewGuid());

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task Create_ValidRequest_ReturnsCreated()
        {
            var instructorId = Guid.NewGuid();
            SetUserWithId(instructorId);

            var courseDto = new CourseDto { Title = "Test", InstructorId = instructorId };
            _mockCourseService.Setup(s => s.CreateAsync(courseDto, instructorId)).ReturnsAsync(new CourseDto { Id = Guid.NewGuid(), Title = "Test", InstructorId = instructorId });

            var result = await _controller.Create(courseDto);

            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
        }

        [Test]
        public async Task Update_ValidId_ReturnsOk()
        {
            _mockCourseService.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CourseDto>())).ReturnsAsync(true);

            var result = await _controller.Update(Guid.NewGuid(), new CourseDto());

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task Update_InvalidId_ReturnsNotFound()
        {
            _mockCourseService.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CourseDto>())).ReturnsAsync(false);

            var result = await _controller.Update(Guid.NewGuid(), new CourseDto());

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task Delete_ValidId_ReturnsOk()
        {
            _mockCourseService.Setup(s => s.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            var result = await _controller.Delete(Guid.NewGuid());

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task Delete_InvalidId_ReturnsNotFound()
        {
            _mockCourseService.Setup(s => s.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await _controller.Delete(Guid.NewGuid());

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetCoursesByInstructor_ValidRequest_ReturnsOk()
        {
            var instructorId = Guid.NewGuid();
            SetUserWithId(instructorId);
            _mockCourseService.Setup(s => s.GetByInstructorIdAsync(instructorId, 1, 10)).ReturnsAsync(new List<CourseDto>());

            var result = await _controller.GetCoursesByInstructor();

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetStudentsEnrolledInCourse_ValidRequest_ReturnsOk()
        {
            var instructorId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            SetUserWithId(instructorId);
            var courseDto = new CourseDto { Id = courseId, InstructorId = instructorId };
            _mockCourseService.Setup(s => s.GetByIdAsync(courseId)).ReturnsAsync(courseDto);
            _mockEnrollmentService.Setup(s => s.GetStudentsEnrolledInCourseAsync(courseId)).ReturnsAsync(new List<UserDtoResponse>());

            var result = await _controller.GetStudentsEnrolledInCourse(courseId);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task SearchCourses_ValidQuery_ReturnsOk()
        {
            _mockCourseService.Setup(s => s.SearchByNameAsync("test")).ReturnsAsync(new List<CourseDto>());

            var result = await _controller.SearchCourses("test");

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
    }
}
