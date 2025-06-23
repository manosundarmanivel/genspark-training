using ElearnAPI.DTOs;
using ElearnAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElearnAPI.Interfaces.Services
{
    public interface IUserService
    {
        // Methods returning DTOs (for API responses)
        Task<UserDto?> GetByIdAsync(Guid id);
        Task<UserDto?> GetByUsernameAsync(string username);
        Task<IEnumerable<UserDto>> GetAllAsync(int page, int pageSize);
        Task<UserDto> CreateAsync(UserDto userDto, string password);
        Task<bool> UpdateAsync(Guid id, UserDto userDto);
        Task<bool> DeleteAsync(Guid id);

        // Methods returning full User model (for internal logic & updates)
        Task<User?> GetUserModelByUsernameAsync(string username);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);

        // Method to update refresh token info on User model
        Task UpdateRefreshTokenAsync(User user);

        Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
    

    }
}
