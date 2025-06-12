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
    public class CourseServiceTests
    {
        private Mock<ICourseRepository> _courseRepoMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private CourseService _courseService = null!;

        [SetUp]
        public void Setup()
        {
            _courseRepoMock = new Mock<ICourseRepository>();
            _mapperMock = new Mock<IMapper>();
            _courseService = new CourseService(_courseRepoMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task CreateAsync_ShouldMapAndAddCourse()
        {
            var instructorId = Guid.NewGuid();
            var dto = new CourseDto { Title = "Test", Description = "Desc" };
            var entity = new Course { Title = "Test", Description = "Desc", InstructorId = instructorId };

            _mapperMock.Setup(m => m.Map<Course>(dto)).Returns(entity);
            _mapperMock.Setup(m => m.Map<CourseDto>(entity)).Returns(dto);

            var result = await _courseService.CreateAsync(dto, instructorId);

            Assert.That(result, Is.EqualTo(dto));
            _courseRepoMock.Verify(r => r.AddAsync(entity), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_CourseExists_ReturnsTrue()
        {
            var courseId = Guid.NewGuid();
            var course = new Course { Id = courseId };

            _courseRepoMock.Setup(r => r.GetByIdAsync(courseId)).ReturnsAsync(course);

            var result = await _courseService.DeleteAsync(courseId);

            Assert.IsTrue(result);
            _courseRepoMock.Verify(r => r.DeleteAsync(course), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_CourseDoesNotExist_ReturnsFalse()
        {
            var courseId = Guid.NewGuid();
            _courseRepoMock.Setup(r => r.GetByIdAsync(courseId)).ReturnsAsync((Course?)null);

            var result = await _courseService.DeleteAsync(courseId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetAllAsync_ReturnsMappedDtos()
        {
            var courses = new List<Course>
            {
                new Course { Title = "C1" },
                new Course { Title = "C2" }
            };
            var dtos = new List<CourseDto>
            {
                new CourseDto { Title = "C1" },
                new CourseDto { Title = "C2" }
            };

            _courseRepoMock.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync(courses);
            _mapperMock.Setup(m => m.Map<IEnumerable<CourseDto>>(courses)).Returns(dtos);

            var result = await _courseService.GetAllAsync(1, 10);

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_CourseExists_ReturnsDto()
        {
            var id = Guid.NewGuid();
            var course = new Course { Id = id, Title = "T" };
            var dto = new CourseDto { Title = "T" };

            _courseRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(course);
            _mapperMock.Setup(m => m.Map<CourseDto>(course)).Returns(dto);

            var result = await _courseService.GetByIdAsync(id);

            Assert.That(result?.Title, Is.EqualTo("T"));
        }

        [Test]
        public async Task GetByIdAsync_CourseNotFound_ReturnsNull()
        {
            var id = Guid.NewGuid();
            _courseRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Course?)null);

            var result = await _courseService.GetByIdAsync(id);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetByInstructorIdAsync_ReturnsMappedDtos()
        {
            var instructorId = Guid.NewGuid();
            var courses = new List<Course> { new Course { Title = "T" } };
            var dtos = new List<CourseDto> { new CourseDto { Title = "T" } };

            _courseRepoMock.Setup(r => r.GetByInstructorIdAsync(instructorId, 1, 5)).ReturnsAsync(courses);
            _mapperMock.Setup(m => m.Map<IEnumerable<CourseDto>>(courses)).Returns(dtos);

            var result = await _courseService.GetByInstructorIdAsync(instructorId, 1, 5);

            Assert.That(result.First().Title, Is.EqualTo("T"));
        }

        [Test]
        public async Task UpdateAsync_CourseExists_UpdatesAndReturnsTrue()
        {
            var id = Guid.NewGuid();
            var course = new Course { Id = id, Title = "Old" };
            var dto = new CourseDto { Title = "New", Description = "Updated" };

            _courseRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(course);

            var result = await _courseService.UpdateAsync(id, dto);

            Assert.IsTrue(result);
            Assert.That(course.Title, Is.EqualTo("New"));
            _courseRepoMock.Verify(r => r.UpdateAsync(course), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_CourseNotFound_ReturnsFalse()
        {
            var id = Guid.NewGuid();
            var dto = new CourseDto { Title = "New" };

            _courseRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Course?)null);

            var result = await _courseService.UpdateAsync(id, dto);

            Assert.IsFalse(result);
        }
    }
}
