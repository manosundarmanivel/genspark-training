using NotifyAPI.Models;
using NotifyAPI.Services;
using NotifyAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotifyAPI.DTOs;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;

    public AuthController(IUserService userService, IJwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userService.AuthenticateAsync(request.Username, request.Password);
        if (user == null) return Unauthorized();
        var token = _jwtService.GenerateToken(user);
        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var user = await _userService.RegisterAsync(request.Username, request.Password, request.Role);
            return Ok("User registered successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}