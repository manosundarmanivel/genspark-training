using ElearnAPI.Controllers;
using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElearnAPI.Tests.Controllers
{
    public class UsersControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private UsersController _controller;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UsersController(_userServiceMock.Object);
        }

        private Role GetTestRole(string roleName = "Admin")
        {
            return new Role { Id = 1, Name = roleName };
        }

        [Test]
        public async Task GetAll_ReturnsOkResultWithUsers()
        {
            var users = new List<UserDto>
            {
                new UserDto { Id = Guid.NewGuid(), Username = "user1", Role = GetTestRole() }
            };

            _userServiceMock.Setup(s => s.GetAllAsync(1, 10)).ReturnsAsync(users);

            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.IsTrue((bool)okResult.Value!.GetType().GetProperty("success")!.GetValue(okResult.Value)!);
        }

        [Test]
        public async Task GetById_UserExists_ReturnsOk()
        {
            var user = new UserDto { Id = Guid.NewGuid(), Username = "john", Role = GetTestRole() };
            _userServiceMock.Setup(s => s.GetByIdAsync(user.Id)).ReturnsAsync(user);

            var result = await _controller.GetById(user.Id);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetById_UserNotExists_ReturnsNotFound()
        {
            _userServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((UserDto?)null);

            var result = await _controller.GetById(Guid.NewGuid());

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task Create_UsernameAlreadyExists_ReturnsConflict()
        {
            var dto = new CreateUserDto { Username = "admin", Password = "pass", Role = GetTestRole() };
            _userServiceMock.Setup(s => s.GetByUsernameAsync(dto.Username)).ReturnsAsync(new UserDto());

            var result = await _controller.Create(dto);

            Assert.IsInstanceOf<ConflictObjectResult>(result);
        }

        [Test]
        public async Task Create_ValidUser_ReturnsCreated()
        {
            var dto = new CreateUserDto { Username = "newuser", Password = "pass", Role = GetTestRole() };
            var createdUser = new UserDto { Id = Guid.NewGuid(), Username = dto.Username, Role = dto.Role };

            _userServiceMock.Setup(s => s.GetByUsernameAsync(dto.Username)).ReturnsAsync((UserDto?)null);
            _userServiceMock.Setup(s => s.CreateAsync(It.IsAny<UserDto>(), dto.Password)).ReturnsAsync(createdUser);

            var result = await _controller.Create(dto);

            Assert.IsInstanceOf<CreatedAtActionResult>(result);
        }

        [Test]
        public async Task Update_UserExists_ReturnsOk()
        {
            var userDto = new UserDto { Id = Guid.NewGuid(), Username = "update", Role = GetTestRole() };

            _userServiceMock.Setup(s => s.UpdateAsync(userDto.Id, userDto)).ReturnsAsync(true);

            var result = await _controller.Update(userDto.Id, userDto);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Update_UserNotExists_ReturnsNotFound()
        {
            var userDto = new UserDto { Id = Guid.NewGuid(), Username = "update", Role = GetTestRole() };

            _userServiceMock.Setup(s => s.UpdateAsync(userDto.Id, userDto)).ReturnsAsync(false);

            var result = await _controller.Update(userDto.Id, userDto);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task Delete_UserExists_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            _userServiceMock.Setup(s => s.DeleteAsync(userId)).ReturnsAsync(true);

            var result = await _controller.Delete(userId);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Delete_UserNotExists_ReturnsNotFound()
        {
            var userId = Guid.NewGuid();
            _userServiceMock.Setup(s => s.DeleteAsync(userId)).ReturnsAsync(false);

            var result = await _controller.Delete(userId);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
    }
}
