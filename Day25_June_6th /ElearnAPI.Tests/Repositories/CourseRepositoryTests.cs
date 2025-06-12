using ElearnAPI.Data;
using ElearnAPI.Models;
using ElearnAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ElearnAPI.Tests.Repositories
{
    public class CourseRepositoryTests
    {
        private ElearnDbContext _context = null!;
        private CourseRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ElearnDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;

            _context = new ElearnDbContext(options);
            _repository = new CourseRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose(); 
        }

        private Course CreateCourse(Guid? instructorId = null) => new Course
        {
            Id = Guid.NewGuid(),
            Title = "Test Course",
            Description = "Test Description",
            InstructorId = instructorId ?? Guid.NewGuid()
        };

        [Test]
        public async Task AddAsync_Should_Add_Course()
        {
            var course = CreateCourse();
            await _repository.AddAsync(course);
            var result = await _context.Courses.FindAsync(course.Id);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task DeleteAsync_Should_SoftDelete_Course()
        {
            var course = CreateCourse();
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(course);
            var result = await _context.Courses.FindAsync(course.Id);
            Assert.IsTrue(result!.IsDeleted);
        }

        [Test]
        public async Task UpdateAsync_Should_Update_Course()
        {
            var course = CreateCourse();
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            course.Title = "Updated";
            await _repository.UpdateAsync(course);
            var result = await _context.Courses.FindAsync(course.Id);
            Assert.AreEqual("Updated", result!.Title);
        }

        [Test]
        public async Task GetAllAsync_Should_Only_Return_Not_Deleted()
        {
            var c1 = CreateCourse();
            var c2 = CreateCourse(); c2.IsDeleted = true;

            await _context.Courses.AddRangeAsync(c1, c2);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync(1, 10);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_Correct_Course()
        {
            var course = CreateCourse();
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(course.Id);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetByInstructorIdAsync_Should_Filter_By_Instructor()
        {
            var instructorId = Guid.NewGuid();
            var course1 = CreateCourse(instructorId);
            var course2 = CreateCourse(Guid.NewGuid());

            await _context.Courses.AddRangeAsync(course1, course2);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByInstructorIdAsync(instructorId, 1, 10);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task CountAsync_Should_Count_Not_Deleted()
        {
            var c1 = CreateCourse();
            var c2 = CreateCourse(); c2.IsDeleted = true;

            await _context.Courses.AddRangeAsync(c1, c2);
            await _context.SaveChangesAsync();

            var count = await _repository.CountAsync();
            Assert.AreEqual(1, count);
        }
    }
}
