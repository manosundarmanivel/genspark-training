using ElearnAPI.Controllers;
using ElearnAPI.DTOs;
using ElearnAPI.Interfaces;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using AutoMapper;
using System;
using System.Threading.Tasks;

namespace ElearnAPI.Tests.Controllers
{
    public class AuthControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private Mock<IJwtTokenService> _jwtServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IRoleService> _roleServiceMock;
        private AuthController _controller;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _jwtServiceMock = new Mock<IJwtTokenService>();
            _mapperMock = new Mock<IMapper>();
            _roleServiceMock = new Mock<IRoleService>();

            _controller = new AuthController(
                _userServiceMock.Object,
                _jwtServiceMock.Object,
                _mapperMock.Object,
                _roleServiceMock.Object);
        }

        [Test]
        public async Task Register_ReturnsBadRequest_WhenUsernameExists()
        {
            _userServiceMock.Setup(s => s.GetByUsernameAsync("existingUser")).ReturnsAsync(new UserDto());

            var result = await _controller.Register(new RegisterDto { Username = "existingUser" });

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Register_ReturnsBadRequest_WhenRoleIsInvalid()
        {
            _userServiceMock.Setup(s => s.GetByUsernameAsync("newUser")).ReturnsAsync((UserDto)null);
            _roleServiceMock.Setup(r => r.GetByNameAsync("InvalidRole")).ReturnsAsync((Role)null);

            var result = await _controller.Register(new RegisterDto { Username = "newUser", Role = "InvalidRole" });

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Register_ReturnsOk_WhenUserCreated()
        {
            var role = new Role { Name = "Student" };
            _userServiceMock.Setup(s => s.GetByUsernameAsync("newUser")).ReturnsAsync((UserDto)null);
            _roleServiceMock.Setup(r => r.GetByNameAsync("Student")).ReturnsAsync(role);
            _userServiceMock.Setup(s => s.CreateAsync(It.IsAny<UserDto>(), It.IsAny<string>()))
                            .ReturnsAsync(new UserDto { Username = "newUser", Role = role });

            var result = await _controller.Register(new RegisterDto { Username = "newUser", Password = "pass", Role = "Student" });

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Login_ReturnsUnauthorized_WhenInvalidCredentials()
        {
            _userServiceMock.Setup(s => s.GetUserModelByUsernameAsync("user")).ReturnsAsync((User)null);

            var result = await _controller.Login(new LoginDto { Username = "user", Password = "wrong" });

            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
        }

        [Test]
        public async Task Login_ReturnsOk_WhenValidCredentials()
        {
            var user = new User { Username = "user", PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass") };
            _userServiceMock.Setup(s => s.GetUserModelByUsernameAsync("user")).ReturnsAsync(user);
            _jwtServiceMock.Setup(j => j.GenerateAccessToken(user)).Returns("accessToken");
            _jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns("refreshToken");

            var result = await _controller.Login(new LoginDto { Username = "user", Password = "pass" });

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Refresh_ReturnsUnauthorized_WhenInvalidRefreshToken()
        {
            _userServiceMock.Setup(s => s.GetByRefreshTokenAsync("invalidToken")).ReturnsAsync((User)null);

            var result = await _controller.Refresh(new RefreshTokenDto { RefreshToken = "invalidToken" });

            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
        }

        [Test]
        public async Task Refresh_ReturnsOk_WhenValidToken()
        {
            var user = new User { RefreshToken = "refreshToken", RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1) };
            _userServiceMock.Setup(s => s.GetByRefreshTokenAsync("refreshToken")).ReturnsAsync(user);
            _jwtServiceMock.Setup(j => j.GenerateAccessToken(user)).Returns("newAccessToken");
            _jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns("newRefreshToken");

            var result = await _controller.Refresh(new RefreshTokenDto { RefreshToken = "refreshToken" });

            Assert.IsInstanceOf<OkObjectResult>(result);
        }
    }
}
