using NotifyAPI.Interfaces;
using NotifyAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;


namespace NotifyAPI.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context) => _context = context;

       public async Task<User?> AuthenticateAsync(string username, string password)
{
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    if (user == null) return null;

    return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) ? user : null;
}


        public async Task<User> RegisterAsync(string username, string password, string role)
        {
            if (_context.Users.Any(u => u.Username == username))
                throw new Exception("User already exists");

            var user = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}