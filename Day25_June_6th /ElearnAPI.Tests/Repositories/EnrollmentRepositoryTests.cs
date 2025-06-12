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
    public class EnrollmentRepositoryTests
    {
        private ElearnDbContext _context = null!;
        private EnrollmentRepository _repository = null!;
        private Guid _userId;
        private Guid _courseId;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<ElearnDbContext>()
                .UseInMemoryDatabase(databaseName: "EnrollmentDb_" + Guid.NewGuid())
                .Options;

            _context = new ElearnDbContext(options);
            _repository = new EnrollmentRepository(_context);

           
            _userId = Guid.NewGuid();
            _courseId = Guid.NewGuid();

            var user = new User
            {
                Id = _userId,
                Username = "testuser",
             
                PasswordHash = "hashedpw"
            };

            var course = new Course
            {
                Id = _courseId,
                Title = "Test Course",
                Description = "Test Description"
            };

            await _context.Users.AddAsync(user);
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AddEnrollmentAsync_ShouldAddEnrollment()
        {
            var enrollment = new Enrollment
            {
                UserId = _userId,
                CourseId = _courseId,
                EnrolledAt = DateTime.UtcNow
            };

            await _repository.AddEnrollmentAsync(enrollment);

            var exists = await _context.Enrollments.AnyAsync(e => e.UserId == _userId && e.CourseId == _courseId);
            Assert.That(exists, Is.True);
        }

        [Test]
        public async Task IsEnrolledAsync_ShouldReturnTrue_IfUserIsEnrolled()
        {
            var enrollment = new Enrollment
            {
                UserId = _userId,
                CourseId = _courseId
            };

            await _repository.AddEnrollmentAsync(enrollment);

            var isEnrolled = await _repository.IsEnrolledAsync(_userId, _courseId);
            Assert.That(isEnrolled, Is.True);
        }

        [Test]
        public async Task IsEnrolledAsync_ShouldReturnFalse_IfUserIsNotEnrolled()
        {
            var isEnrolled = await _repository.IsEnrolledAsync(_userId, _courseId);
            Assert.That(isEnrolled, Is.False);
        }

        [Test]
        public async Task GetEnrollmentAsync_ShouldReturnEnrollment()
        {
            var enrollment = new Enrollment
            {
                UserId = _userId,
                CourseId = _courseId
            };

            await _repository.AddEnrollmentAsync(enrollment);

            var result = await _repository.GetEnrollmentAsync(_userId, _courseId);
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.UserId, Is.EqualTo(_userId));
        }

        [Test]
        public async Task RemoveEnrollmentAsync_ShouldRemoveEnrollment()
        {
            var enrollment = new Enrollment
            {
                UserId = _userId,
                CourseId = _courseId
            };

            await _repository.AddEnrollmentAsync(enrollment);
            await _repository.RemoveEnrollmentAsync(enrollment);

            var exists = await _repository.IsEnrolledAsync(_userId, _courseId);
            Assert.That(exists, Is.False);
        }

        [Test]
        public async Task GetEnrolledCoursesAsync_ShouldReturnCourses()
        {
            var enrollment = new Enrollment
            {
                UserId = _userId,
                CourseId = _courseId
            };

            await _repository.AddEnrollmentAsync(enrollment);

            var courses = await _repository.GetEnrolledCoursesAsync(_userId);
            Assert.That(courses.Count(), Is.EqualTo(1));
            Assert.That(courses.First().Id, Is.EqualTo(_courseId));
        }

        [Test]
        public async Task GetEnrollmentsByCourseIdAsync_ShouldReturnStudents()
        {
            var enrollment = new Enrollment
            {
                UserId = _userId,
                CourseId = _courseId
            };

            await _repository.AddEnrollmentAsync(enrollment);

            var enrollments = await _repository.GetEnrollmentsByCourseIdAsync(_courseId);
            Assert.That(enrollments.Count(), Is.EqualTo(1));
            Assert.That(enrollments.First().UserId, Is.EqualTo(_userId));
        }
    }
}
