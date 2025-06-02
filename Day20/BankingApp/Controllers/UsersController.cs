using BankingApp.Auth;
using BankingApp.DTOs;
using BankingApp.Models;
using BankingApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
    

        public UsersController(IUserService userService)
        {
            _userService = userService;
            
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            var user = await _userService.RegisterAsync(dto);
            if (user == null)
                return BadRequest("User already exists.");

            return Ok(new { message = "Registration successful." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var token = await _userService.LoginAsync(dto);
            if (token == null)
                return Unauthorized("Invalid credentials.");

            
            return Ok(new { token });
        }
    }
}
