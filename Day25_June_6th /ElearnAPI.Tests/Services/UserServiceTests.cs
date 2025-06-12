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
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private UserService _userService = null!;

        private Role StudentRole = new Role { Id = 1, Name = "Student" };
        private Role AdminRole = new Role { Id = 2, Name = "Admin" };
        private Role InstructorRole = new Role { Id = 3, Name = "Instructor" };

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _userService = new UserService(_userRepositoryMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task CreateAsync_ShouldHashPasswordAndReturnUserDto()
        {
            var userDto = new UserDto { Username = "john", Role = StudentRole };
            var userModel = new User { Username = "john", Role = StudentRole };

            _mapperMock.Setup(m => m.Map<User>(userDto)).Returns(userModel);
            _mapperMock.Setup(m => m.Map<UserDto>(userModel)).Returns(userDto);

            var result = await _userService.CreateAsync(userDto, "password123");

            Assert.That(BCrypt.Net.BCrypt.Verify("password123", userModel.PasswordHash));
            Assert.That(result.Username, Is.EqualTo("john"));
        }

        [Test]
        public async Task DeleteAsync_UserExists_ReturnsTrue()
        {
            var id = Guid.NewGuid();
            var user = new User { Id = id };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.DeleteAsync(user)).Returns(Task.CompletedTask);

            var result = await _userService.DeleteAsync(id);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task DeleteAsync_UserDoesNotExist_ReturnsFalse()
        {
            var id = Guid.NewGuid();

            _userRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((User?)null);

            var result = await _userService.DeleteAsync(id);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnMappedUserDtos()
        {
            var users = new List<User> { new User { Username = "john", Role = StudentRole } };
            var userDtos = new List<UserDto> { new UserDto { Username = "john", Role = StudentRole } };

            _userRepositoryMock.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

            var result = await _userService.GetAllAsync(1, 10);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Username, Is.EqualTo("john"));
        }

        [Test]
        public async Task GetByIdAsync_UserExists_ReturnsUserDto()
        {
            var id = Guid.NewGuid();
            var user = new User { Id = id, Username = "john", Role = StudentRole };
            var userDto = new UserDto { Username = "john", Role = StudentRole };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

            var result = await _userService.GetByIdAsync(id);

            Assert.That(result?.Username, Is.EqualTo("john"));
        }

        [Test]
        public async Task GetByIdAsync_UserDoesNotExist_ReturnsNull()
        {
            _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

            var result = await _userService.GetByIdAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }

        [Test]
        public async Task UpdateAsync_UserExists_ShouldUpdateAndReturnTrue()
        {
            var id = Guid.NewGuid();
            var user = new User { Id = id, Username = "old", Role = InstructorRole };
            var userDto = new UserDto { Username = "new", Role = AdminRole };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            var result = await _userService.UpdateAsync(id, userDto);

            Assert.IsTrue(result);
            _userRepositoryMock.Verify(r => r.UpdateAsync(It.Is<User>(u =>
                u.Username == "new" &&
                u.Role == AdminRole)), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_UserDoesNotExist_ReturnsFalse()
        {
            _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

            var result = await _userService.UpdateAsync(Guid.NewGuid(), new UserDto());

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetByUsernameAsync_UserExists_ReturnsUserDto()
        {
            var user = new User { Username = "john", Role = StudentRole };
            var userDto = new UserDto { Username = "john", Role = StudentRole };

            _userRepositoryMock.Setup(r => r.GetByUsernameAsync("john")).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

            var result = await _userService.GetByUsernameAsync("john");

            Assert.That(result?.Username, Is.EqualTo("john"));
        }

        [Test]
        public async Task GetUserModelByUsernameAsync_ShouldReturnUser()
        {
            var user = new User { Username = "john", Role = StudentRole };

            _userRepositoryMock.Setup(r => r.GetByUsernameAsync("john")).ReturnsAsync(user);

            var result = await _userService.GetUserModelByUsernameAsync("john");

            Assert.That(result?.Username, Is.EqualTo("john"));
        }

        [Test]
        public async Task GetByRefreshTokenAsync_ShouldReturnUser()
        {
            var user = new User { Username = "john", Role = StudentRole };

            _userRepositoryMock.Setup(r => r.GetByRefreshTokenAsync("token")).ReturnsAsync(user);

            var result = await _userService.GetByRefreshTokenAsync("token");

            Assert.That(result?.Username, Is.EqualTo("john"));
        }

        [Test]
        public async Task UpdateRefreshTokenAsync_ShouldCallUpdate()
        {
            var user = new User { Username = "john", Role = StudentRole };

            await _userService.UpdateRefreshTokenAsync(user);

            _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }
    }
}
