using System.ComponentModel.DataAnnotations;
namespace ElearnAPI.DTOs
{
    public class RefreshTokenDto
    {
        [Required(ErrorMessage = "Refresh token is required.")]
        public string RefreshToken { get; set; } = null!;
    }
}
