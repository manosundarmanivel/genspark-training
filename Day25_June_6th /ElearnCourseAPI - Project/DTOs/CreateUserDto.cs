using ElearnAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace ElearnAPI.DTOs
{
    public class CreateUserDto
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public Role Role { get; set; } = null!;
    }
}