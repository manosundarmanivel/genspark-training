
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BankingApp.Auth;
using BankingApp.DTOs;
using BankingApp.Models;
using BankingApp.Repositories.Interfaces;
using BankingApp.Services.Interfaces;

namespace BankingApp.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public UserService(IUserRepository userRepo, IJwtTokenGenerator tokenGenerator)
        {
            _userRepo = userRepo;
            _tokenGenerator = tokenGenerator;
        }

public async Task<string> RegisterAsync(UserRegisterDto dto)
{
    if (await _userRepo.GetByUsernameAsync(dto.Username) != null)
        return null; 

    using var hmac = new HMACSHA512();
    var user = new User
    {
        Username = dto.Username,
        Email = dto.Email,
        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
        PasswordSalt = hmac.Key
    };

    await _userRepo.AddAsync(user);
    return _tokenGenerator.GenerateToken(user);
}


public async Task<string> LoginAsync(UserLoginDto dto)
{
    var user = await _userRepo.GetByUsernameAsync(dto.Username);
    if (user == null)
        return null;

    using var hmac = new HMACSHA512(user.PasswordSalt);
    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

    for (int i = 0; i < computedHash.Length; i++)
        if (computedHash[i] != user.PasswordHash[i])
            return null;

    return _tokenGenerator.GenerateToken(user);
}


        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _userRepo.GetByIdAsync(id);
        }
    }
}
