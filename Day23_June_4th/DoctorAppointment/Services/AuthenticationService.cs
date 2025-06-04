
using DoctorAppointment.Interfaces;
using DoctorAppointment.Models;
using DoctorAppointment.Models.DTO;
using DoctorAppointment.Models.DTOs.DoctorSpecialities;
using Microsoft.Extensions.Logging;
using Google.Apis.Auth; 

namespace FirstAPI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ITokenService _tokenService;
        private readonly IEncryptionService _encryptionService;
        private readonly IRepository<string, User> _userRepository;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(ITokenService tokenService,
                                    IEncryptionService encryptionService,
                                    IRepository<string, User> userRepository,
                                    ILogger<AuthenticationService> logger)
        {
            _tokenService = tokenService;
            _encryptionService = encryptionService;
            _userRepository = userRepository;
            _logger = logger;
        }
        public async Task<UserLoginResponse> Login(UserLoginRequest user)
        {
            var dbUser = await _userRepository.Get(user.Username);
            if (dbUser == null)
            {
                _logger.LogCritical("User not found");
                throw new Exception("No such user");
            }
            var encryptedData = await _encryptionService.EncryptData(new EncryptModel
            {
                Data = user.Password,
                HashKey = dbUser.HashKey
            });
            for (int i = 0; i < encryptedData.EncryptedData.Length; i++)
            {
                if (encryptedData.EncryptedData[i] != dbUser.Password[i])
                {
                    _logger.LogError("Invalid login attempt");
                    throw new Exception("Invalid password");
                }
            }
            var token = await _tokenService.GenerateToken(dbUser);
            return new UserLoginResponse
            {
                Username = user.Username,
                Token = token,
            };
        }


public async Task<UserLoginResponse> GoogleLogin(string idToken)
{
    try
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new[] { "407408718192.apps.googleusercontent.com" } 
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

        if (payload == null || string.IsNullOrEmpty(payload.Email))
            throw new Exception("Invalid Google token");

        _logger.LogInformation($"Google token validated. Email: {payload.Email}");

        var user = await _userRepository.Get(payload.Email);
        if (user == null)
        {
            _logger.LogCritical("User not found");
            throw new Exception("No such user ");
        }

        var token = await _tokenService.GenerateToken(user);

        return new UserLoginResponse
        {
            Username = user.Username,
            Token = token
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Google token validation failed: {ex.Message}");
        throw new Exception("Google authentication failed");
    }
}


    }
}