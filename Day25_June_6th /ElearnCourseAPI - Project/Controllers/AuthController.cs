using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AutoMapper;
using ElearnAPI.Interfaces;  

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

public AuthController(IUserService userService, IJwtTokenService jwtService, IMapper mapper, IRoleService roleService)
{
    _userService = userService;
    _jwtService = jwtService;
    _mapper = mapper;
    _roleService = roleService;
}



[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterDto dto)
{
    var existingUser = await _userService.GetByUsernameAsync(dto.Username);
    if (existingUser != null)
        return BadRequest(new { success = false, message = "Username already taken" });

    var role = await _roleService.GetByNameAsync(dto.Role);
    if (role == null)
        return BadRequest(new { success = false, message = "Invalid role specified" });

    var userDto = new UserDto
    {
        Username = dto.Username,
        Role = role
    };

    var createdUserDto = await _userService.CreateAsync(userDto, dto.Password);

    if (createdUserDto == null)
        return StatusCode(500, new { success = false, message = "User creation failed" });

    return Ok(new { success = true, data = createdUserDto });
}




        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // Get full User model (not UserDto)
            var user = await _userService.GetUserModelByUsernameAsync(dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { success = false, message = "Invalid credentials" });

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userService.UpdateRefreshTokenAsync(user);

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


    }
}
