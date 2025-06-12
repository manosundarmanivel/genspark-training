using System.ComponentModel.DataAnnotations;
namespace ElearnAPI.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required.")]
        
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters.")]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; } = null!;
    }
}
