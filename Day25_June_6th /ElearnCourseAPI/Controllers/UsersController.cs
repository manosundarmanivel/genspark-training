using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
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
        private readonly Serilog.ILogger _logger;

        public UsersController(IUserService userService)
        {
            _userService = userService;
            _logger = Log.ForContext<UsersController>();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var users = await _userService.GetAllAsync(page, pageSize);
            _logger.Information("Fetched user list: Page={Page}, PageSize={PageSize}", page, pageSize);
            return Ok(new { success = true, data = users });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                _logger.Warning("User not found: {UserId}", id);
                return NotFound(new { success = false, message = "User not found." });
            }

            _logger.Information("Fetched user details: {UserId}", id);
            return Ok(new { success = true, data = user });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.Warning("Invalid user creation attempt. ModelState errors: {@Errors}", ModelState);
                return BadRequest(new { success = false, message = "Invalid data." });
            }

            var existingUser = await _userService.GetByUsernameAsync(dto.Username);
            if (existingUser != null)
            {
                _logger.Warning("Username already exists: {Username}", dto.Username);
                return Conflict(new { success = false, message = "Username already exists." });
            }

            var userDto = new UserDto
            {
                Username = dto.Username,
                Role = dto.Role
            };

            var user = await _userService.CreateAsync(userDto, dto.Password);
            _logger.Information("User created: {UserId}, Username: {Username}, Role: {Role}", user.Id, user.Username, user.Role);

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, new { success = true, data = user });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.Warning("Invalid user update attempt. UserId={UserId}, ModelState: {@Errors}", id, ModelState);
                return BadRequest(new { success = false, message = "Invalid data." });
            }

            var result = await _userService.UpdateAsync(id, userDto);
            if (!result)
            {
                _logger.Warning("User not found for update: {UserId}", id);
                return NotFound(new { success = false, message = "User not found." });
            }

            _logger.Information("User updated: {UserId}", id);
            return Ok(new { success = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.DeleteAsync(id);
            if (!result)
            {
                _logger.Warning("User not found for deletion: {UserId}", id);
                return NotFound(new { success = false, message = "User not found." });
            }

            _logger.Information("User deleted: {UserId}", id);
            return Ok(new { success = true });
        }
    }
}
