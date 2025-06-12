// ElearnAPI.Tests/Repositories/UserRepositoryTests.cs
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
    public class UserRepositoryTests
    {
        private ElearnDbContext _context;
        private UserRepository _repository;
        private Role _testRole;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<ElearnDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ElearnDbContext(options);
            _repository = new UserRepository(_context);

            _testRole = new Role
            {
                Id = 1,
                Name = "TestRole"
            };

            await _context.Roles.AddAsync(_testRole);
            await _context.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AddAsync_AddsUserSuccessfully()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                PasswordHash = "hash",
                IsDeleted = false,
                Role = _testRole
            };

            await _repository.AddAsync(user);

            var result = await _context.Users.FindAsync(user.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Username, Is.EqualTo("testuser"));
        }

        [Test]
        public async Task GetAllAsync_ReturnsOnlyNonDeletedUsers()
        {
            var user1 = new User
            {
                Id = Guid.NewGuid(),
                Username = "user1",
                PasswordHash = "hash1",
                IsDeleted = false,
                Role = _testRole
            };
            var user2 = new User
            {
                Id = Guid.NewGuid(),
                Username = "user2",
                PasswordHash = "hash2",
                IsDeleted = true,
                Role = _testRole
            };

            await _context.Users.AddRangeAsync(user1, user2);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync(1, 10);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Username, Is.EqualTo("user1"));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsCorrectUser()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "uniqueuser",
                PasswordHash = "hash",
                IsDeleted = false,
                Role = _testRole
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(user.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Username, Is.EqualTo("uniqueuser"));
        }

        [Test]
        public async Task GetByUsernameAsync_ReturnsCorrectUser()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "findme",
                PasswordHash = "hash",
                IsDeleted = false,
                Role = _testRole
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByUsernameAsync("findme");
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Username, Is.EqualTo("findme"));
        }

        [Test]
        public async Task DeleteAsync_SetsIsDeletedToTrue()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "tobedeleted",
                PasswordHash = "hash",
                IsDeleted = false,
                Role = _testRole
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(user);

            var result = await _context.Users.FindAsync(user.Id);
            Assert.That(result!.IsDeleted, Is.True);
        }

        [Test]
        public async Task GetByRefreshTokenAsync_ReturnsValidUser()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "refreshuser",
                PasswordHash = "hash",
                RefreshToken = "validtoken",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(1),
                IsDeleted = false,
                Role = _testRole
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByRefreshTokenAsync("validtoken");
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Username, Is.EqualTo("refreshuser"));
        }

        [Test]
        public async Task CountAsync_ReturnsCorrectCount()
        {
            var user1 = new User
            {
                Id = Guid.NewGuid(),
                Username = "u1",
                PasswordHash = "h1",
                IsDeleted = false,
                Role = _testRole
            };
            var user2 = new User
            {
                Id = Guid.NewGuid(),
                Username = "u2",
                PasswordHash = "h2",
                IsDeleted = true,
                Role = _testRole
            };

            await _context.Users.AddRangeAsync(user1, user2);
            await _context.SaveChangesAsync();

            var count = await _repository.CountAsync();
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateAsync_UpdatesUserCorrectly()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "beforeupdate",
                PasswordHash = "hash",
                IsDeleted = false,
                Role = _testRole
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            user.Username = "afterupdate";
            await _repository.UpdateAsync(user);

            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.That(updatedUser!.Username, Is.EqualTo("afterupdate"));
        }
    }
}
