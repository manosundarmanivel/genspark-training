// Services/UserService.cs
using AutoMapper;
using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Repositories;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElearnAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> CreateAsync(UserDto userDto, string password)
        {
            var user = _mapper.Map<User>(userDto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            await _userRepository.AddAsync(user);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;
            await _userRepository.DeleteAsync(user);
            return true;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(int page, int pageSize)
        {
            var users = await _userRepository.GetAllAsync(page, pageSize);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }


        public async Task<IEnumerable<UserDto>> GetAllAsyncAdmin(int page, int pageSize)
        {
            var users = await _userRepository.GetAllAsyncAdmin(page, pageSize);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> GetByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return null;
            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> UpdateAsync(Guid id, UserDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            user.Username = userDto.Username;
            user.Role = userDto.Role;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<User?> GetUserModelByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _userRepository.GetByRefreshTokenAsync(refreshToken);
        }

        public async Task UpdateRefreshTokenAsync(User user)
        {
            await _userRepository.UpdateAsync(user);
        }

public async Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
{
    var user = await _userRepository.GetByIdAsync(userId);
    if (user == null) return false;

    user.FullName = dto.FullName ?? user.FullName;
    user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
    user.Bio = dto.Bio ?? user.Bio;

    // Save uploaded profile picture if present
    if (dto.ProfilePictureUrl != null && dto.ProfilePictureUrl.Length > 0)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "ProfilePictures");
        Directory.CreateDirectory(uploadsFolder);

        var sanitizedFileName = Path.GetFileName(dto.ProfilePictureUrl.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}_{sanitizedFileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.ProfilePictureUrl.CopyToAsync(stream);
        }

        // Save public relative URL for frontend to use
        user.ProfilePictureUrl = $"/uploads/profilepictures/{uniqueFileName}";
    }

    await _userRepository.UpdateAsync(user);
    return true;
}



        public async Task<bool> SetActiveStatusAsync(Guid id, bool isActive)
{

    var user = await _userRepository.GetByIdAsyncAdmin(id);
    if (user == null) return false;

    user.IsDeleted = isActive;
    user.UpdatedAt = DateTime.UtcNow;

    await _userRepository.UpdateAsync(user);
    return true;
}


    }
}
