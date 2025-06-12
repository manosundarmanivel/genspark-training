using ElearnAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace ElearnAPI.DTOs
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Role is required.")]
        public Role Role { get; set; } = null!;
    }
}