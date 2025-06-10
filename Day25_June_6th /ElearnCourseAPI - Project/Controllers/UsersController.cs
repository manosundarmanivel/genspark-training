// Controllers/UsersController.cs
using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ElearnAPI.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var users = await _userService.GetAllAsync(page, pageSize);
            return Ok(new { success = true, data = users });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound(new { success = false, message = "User not found." });
            return Ok(new { success = true, data = user });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data." });

           
            var existingUser = await _userService.GetByUsernameAsync(dto.Username);
            if (existingUser != null)
                return Conflict(new { success = false, message = "Username already exists." });

            var userDto = new UserDto
            {
                Username = dto.Username,
                Role = dto.Role
            };

            var user = await _userService.CreateAsync(userDto, dto.Password);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, new { success = true, data = user });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data." });

            var result = await _userService.UpdateAsync(id, userDto);
            if (!result) return NotFound(new { success = false, message = "User not found." });
            return Ok(new { success = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.DeleteAsync(id);
            if (!result) return NotFound(new { success = false, message = "User not found." });
            return Ok(new { success = true });
        }
    }
}
