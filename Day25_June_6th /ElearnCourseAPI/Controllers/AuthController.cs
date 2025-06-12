using AutoMapper;
using ElearnAPI.DTOs;
using ElearnAPI.Interfaces;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog; // Serilog logger
using System;
using System.Threading.Tasks;

namespace ElearnAPI.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtService;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        private readonly Serilog.ILogger _logger;

        public AuthController(
            IUserService userService,
            IJwtTokenService jwtService,
            IMapper mapper,
            IRoleService roleService)
        {
            _userService = userService;
            _jwtService = jwtService;
            _mapper = mapper;
            _roleService = roleService;
            _logger = Log.ForContext<AuthController>();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            _logger.Information("Registering user {Username} with role {Role}", dto.Username, dto.Role);

            var existingUser = await _userService.GetByUsernameAsync(dto.Username);
            if (existingUser != null)
            {
                _logger.Warning("Registration failed: Username {Username} already exists", dto.Username);
                return BadRequest(new { success = false, message = "Username already taken" });
            }

            var role = await _roleService.GetByNameAsync(dto.Role);
            if (role == null)
            {
                _logger.Warning("Registration failed: Invalid role {Role} specified", dto.Role);
                return BadRequest(new { success = false, message = "Invalid role specified" });
            }

            var userDto = new UserDto
            {
                Username = dto.Username,
                Role = role
            };

            var createdUserDto = await _userService.CreateAsync(userDto, dto.Password);

            if (createdUserDto == null)
            {
                _logger.Error("User creation failed for {Username}", dto.Username);
                return StatusCode(500, new { success = false, message = "User creation failed" });
            }

            _logger.Information("User {Username} registered successfully", dto.Username);
            return Ok(new { success = true, data = createdUserDto });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            _logger.Information("Login attempt for user {Username}", dto.Username);

            var user = await _userService.GetUserModelByUsernameAsync(dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                _logger.Warning("Login failed for user {Username}", dto.Username);
                return Unauthorized(new { success = false, message = "Invalid credentials" });
            }

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userService.UpdateRefreshTokenAsync(user);

            _logger.Information("User {Username} logged in successfully", dto.Username);
            return Ok(new
            {
                success = true,
                token = accessToken,
                refreshToken = refreshToken
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
        {
            _logger.Information("Refresh token request");

            var user = await _userService.GetByRefreshTokenAsync(dto.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                _logger.Warning("Invalid or expired refresh token");
                return Unauthorized(new { success = false, message = "Invalid refresh token" });
            }

            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userService.UpdateRefreshTokenAsync(user);

            _logger.Information("Refresh token generated successfully for user {UserId}", user.Id);
            return Ok(new
            {
                success = true,
                token = newAccessToken,
                refreshToken = newRefreshToken
            });
        }
    }
}
