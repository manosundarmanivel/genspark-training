using ElearnAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace ElearnAPI.DTOs
{
    public class CreateUserDto
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        [Required]
        public Role Role { get; set; } = null!;

        // Optional profile info
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
