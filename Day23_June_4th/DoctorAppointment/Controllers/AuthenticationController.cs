
using DoctorAppointment.Interfaces;
using DoctorAppointment.Models.DTO;
using DoctorAppointment.Models.DTOs.DoctorSpecialities;
using Microsoft.AspNetCore.Mvc;



namespace DoctorAppointment.Controllers
{


    [ApiController]
    [Route("/api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly Interfaces.IAuthenticationService _authenticationService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IAuthenticationService authenticationService, ILogger<AuthenticationController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }
        [HttpPost]
        public async Task<ActionResult<UserLoginResponse>> UserLogin(UserLoginRequest loginRequest)
        {
            try
            {
                var result = await _authenticationService.Login(loginRequest);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Unauthorized(e.Message);
            }
        }

        [HttpPost("google-login")]
        public async Task<ActionResult<UserLoginResponse>> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                var result = await _authenticationService.GoogleLogin(request.IdToken);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Google login failed");
                return Unauthorized(new { message = e.Message });
            }
        }
    }
}