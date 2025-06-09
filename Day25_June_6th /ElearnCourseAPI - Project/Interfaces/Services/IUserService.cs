using ElearnAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElearnAPI.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserDto?> GetByIdAsync(Guid id);
        Task<UserDto?> GetByUsernameAsync(string username);
        Task<IEnumerable<UserDto>> GetAllAsync(int page, int pageSize);
        Task<UserDto> CreateAsync(UserDto userDto, string password);
        Task<bool> UpdateAsync(Guid id, UserDto userDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
