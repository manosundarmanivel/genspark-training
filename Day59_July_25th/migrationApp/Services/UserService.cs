using ChienVHShopOnline.Dtos;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;

namespace ChienVHShopOnline.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _repo.GetAllAsync();
            return users.Select(u => new UserDto
            {
                UserId = u.UserId,
                Username = u.Username
            });
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return null;

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username
            };
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Password = dto.Password // TODO: Hash password before storing
            };

            await _repo.AddAsync(user);
            await _repo.SaveChangesAsync();

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username
            };
        }

        public async Task<UserDto?> UpdateAsync(UpdateUserDto dto)
        {
            var user = await _repo.GetByIdAsync(dto.UserId);
            if (user == null) return null;

            user.Username = dto.Username;
            user.Password = dto.Password; 
            _repo.Update(user);
            await _repo.SaveChangesAsync();

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return false;

            _repo.Delete(user);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
