using ElearnAPI.DTOs;
using ElearnAPI.Interfaces;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElearnAPI.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtService;

        public AuthController(IUserService userService, IJwtTokenService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userService.GetByUsernameAsync(dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { success = false, message = "Invalid credentials" });

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userService.UpdateRefreshTokenAsync(user); // save refresh info

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
            var user = await _userService.GetByRefreshTokenAsync(dto.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return Unauthorized(new { success = false, message = "Invalid refresh token" });

            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userService.UpdateRefreshTokenAsync(user);

            return Ok(new
            {
                success = true,
                token = newAccessToken,
                refreshToken = newRefreshToken
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity?.Name;
            if (username == null) return Unauthorized();

            var user = await _userService.GetByUsernameAsync(username);
            if (user == null) return Unauthorized();

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userService.UpdateRefreshTokenAsync(user);

            return Ok(new { success = true, message = "Logged out" });
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var username = User.Identity?.Name;
            if (username == null) return Unauthorized();

            var user = await _userService.GetByUsernameAsync(username);
            return Ok(new { success = true, data = user });
        }
    }
}
