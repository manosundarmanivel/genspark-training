using AutoMapper;
using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Repositories;
using ElearnAPI.Models;
using ElearnAPI.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElearnAPI.Tests.Services
{
    public class EnrollmentServiceTests
    {
        private Mock<IEnrollmentRepository> _enrollmentRepoMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private EnrollmentService _enrollmentService = null!;

        [SetUp]
        public void Setup()
        {
            _enrollmentRepoMock = new Mock<IEnrollmentRepository>();
            _mapperMock = new Mock<IMapper>();
            _enrollmentService = new EnrollmentService(_enrollmentRepoMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task EnrollStudentAsync_AlreadyEnrolled_ReturnsFalse()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _enrollmentRepoMock.Setup(r => r.IsEnrolledAsync(userId, courseId)).ReturnsAsync(true);

            var result = await _enrollmentService.EnrollStudentAsync(userId, courseId);

            Assert.IsFalse(result);
            _enrollmentRepoMock.Verify(r => r.AddEnrollmentAsync(It.IsAny<Enrollment>()), Times.Never);
        }

        [Test]
        public async Task EnrollStudentAsync_NotEnrolled_AddsEnrollmentAndReturnsTrue()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _enrollmentRepoMock.Setup(r => r.IsEnrolledAsync(userId, courseId)).ReturnsAsync(false);

            var result = await _enrollmentService.EnrollStudentAsync(userId, courseId);

            Assert.IsTrue(result);
            _enrollmentRepoMock.Verify(r => r.AddEnrollmentAsync(It.Is<Enrollment>(e =>
                e.UserId == userId && e.CourseId == courseId)), Times.Once);
        }

        [Test]
        public async Task UnenrollStudentAsync_EnrollmentExists_RemovesEnrollmentAndReturnsTrue()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var enrollment = new Enrollment { UserId = userId, CourseId = courseId };

            _enrollmentRepoMock.Setup(r => r.GetEnrollmentAsync(userId, courseId)).ReturnsAsync(enrollment);

            var result = await _enrollmentService.UnenrollStudentAsync(userId, courseId);

            Assert.IsTrue(result);
            _enrollmentRepoMock.Verify(r => r.RemoveEnrollmentAsync(enrollment), Times.Once);
        }

        [Test]
        public async Task UnenrollStudentAsync_EnrollmentNotFound_ReturnsFalse()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _enrollmentRepoMock.Setup(r => r.GetEnrollmentAsync(userId, courseId)).ReturnsAsync((Enrollment?)null);

            var result = await _enrollmentService.UnenrollStudentAsync(userId, courseId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetStudentCoursesAsync_ReturnsCourses()
        {
            var userId = Guid.NewGuid();
            var courses = new List<Course>
            {
                new Course { Id = Guid.NewGuid(), Title = "Math" },
                new Course { Id = Guid.NewGuid(), Title = "Science" }
            };

            _enrollmentRepoMock.Setup(r => r.GetEnrolledCoursesAsync(userId)).ReturnsAsync(courses);

            var result = await _enrollmentService.GetStudentCoursesAsync(userId);

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task IsStudentEnrolledInCourseAsync_ReturnsCorrectStatus()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _enrollmentRepoMock.Setup(r => r.IsEnrolledAsync(userId, courseId)).ReturnsAsync(true);

            var result = await _enrollmentService.IsStudentEnrolledInCourseAsync(userId, courseId);

            Assert.IsTrue(result);
        }

        [Test]
public async Task GetStudentsEnrolledInCourseAsync_ReturnsMappedUserDtos()
{
    // Arrange
    var courseId = Guid.NewGuid();
    var user1 = new User { Id = Guid.NewGuid(), Username = "alice" };
    var user2 = new User { Id = Guid.NewGuid(), Username = "bob" };

    var enrollments = new List<Enrollment>
    {
        new Enrollment { Student = user1 },
        new Enrollment { Student = user2 }
    };

    var userDtoResponses = new List<UserDtoResponse>
    {
        new UserDtoResponse { Id = user1.Id, Username = "alice" },
        new UserDtoResponse { Id = user2.Id, Username = "bob" }
    };

    _enrollmentRepoMock.Setup(r => r.GetEnrollmentsByCourseIdAsync(courseId)).ReturnsAsync(enrollments);
    _mapperMock.Setup(m => m.Map<UserDtoResponse>(user1)).Returns(userDtoResponses[0]);
    _mapperMock.Setup(m => m.Map<UserDtoResponse>(user2)).Returns(userDtoResponses[1]);

  
    var result = (await _enrollmentService.GetStudentsEnrolledInCourseAsync(courseId)).ToList();

    
    Assert.That(result.Count, Is.EqualTo(2));
    Assert.That(result[0].Username, Is.EqualTo("alice"));
    Assert.That(result[1].Username, Is.EqualTo("bob"));
}

    }
}
