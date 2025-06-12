using ElearnAPI.Data;
using ElearnAPI.Models;
using ElearnAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ElearnAPI.Tests.Repositories
{
    public class RoleRepositoryTests
    {
        private ElearnDbContext _context = null!;
        private RoleRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ElearnDbContext>()
                .UseInMemoryDatabase(databaseName: "RoleTestDb_" + System.Guid.NewGuid())
                .Options;

            _context = new ElearnDbContext(options);
            _repository = new RoleRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetByNameAsync_ShouldReturnCorrectRole_WhenRoleExists()
        {
            var role = new Role { Name = "Admin" };
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByNameAsync("Admin");

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Admin"));
        }

        [Test]
        public async Task GetByNameAsync_ShouldReturnNull_WhenRoleDoesNotExist()
        {
            var result = await _repository.GetByNameAsync("NonExistentRole");

            Assert.That(result, Is.Null);
        }
    }
}
