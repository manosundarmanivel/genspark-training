using ElearnAPI.Controllers;
using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ElearnAPI.Models; 


namespace ElearnAPI.Tests.Controllers
{
    [TestFixture]
    public class EnrollmentControllerTests
    {
        private Mock<IEnrollmentService> _enrollmentServiceMock;
        private EnrollmentController _controller;

        [SetUp]
        public void SetUp()
        {
            _enrollmentServiceMock = new Mock<IEnrollmentService>();
            _controller = new EnrollmentController(_enrollmentServiceMock.Object);

            var studentId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, studentId.ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task Enroll_ReturnsOk_WhenEnrollmentSucceeds()
        {
            var courseId = Guid.NewGuid();
            _enrollmentServiceMock.Setup(x => x.EnrollStudentAsync(It.IsAny<Guid>(), courseId))
                                  .ReturnsAsync(true);

            var result = await _controller.Enroll(courseId) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task Enroll_ReturnsBadRequest_WhenEnrollmentFails()
        {
            var courseId = Guid.NewGuid();
            _enrollmentServiceMock.Setup(x => x.EnrollStudentAsync(It.IsAny<Guid>(), courseId))
                                  .ReturnsAsync(false);

            var result = await _controller.Enroll(courseId) as BadRequestObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task Unenroll_ReturnsOk_WhenUnenrollmentSucceeds()
        {
            var courseId = Guid.NewGuid();
            _enrollmentServiceMock.Setup(x => x.UnenrollStudentAsync(It.IsAny<Guid>(), courseId))
                                  .ReturnsAsync(true);

            var result = await _controller.Unenroll(courseId) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task Unenroll_ReturnsNotFound_WhenUnenrollmentFails()
        {
            var courseId = Guid.NewGuid();
            _enrollmentServiceMock.Setup(x => x.UnenrollStudentAsync(It.IsAny<Guid>(), courseId))
                                  .ReturnsAsync(false);

            var result = await _controller.Unenroll(courseId) as NotFoundObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
public async Task GetMyCourses_ReturnsCourses_WhenUserIsValid()
{
    var mockCourses = new List<Course>
    {
        new Course { Id = Guid.NewGuid(), Title = "Course 1", Description = "Desc", InstructorId = Guid.NewGuid() },
        new Course { Id = Guid.NewGuid(), Title = "Course 2", Description = "Desc", InstructorId = Guid.NewGuid() }
    };

    _enrollmentServiceMock.Setup(x => x.GetStudentCoursesAsync(It.IsAny<Guid>()))
                          .ReturnsAsync(mockCourses.AsEnumerable());

    var result = await _controller.GetMyCourses() as OkObjectResult;

    Assert.That(result, Is.Not.Null);
    Assert.That(result.StatusCode, Is.EqualTo(200));
    var data = result.Value?.GetType().GetProperty("data")?.GetValue(result.Value, null);
    Assert.That(data, Is.Not.Null);
}

    }
}
