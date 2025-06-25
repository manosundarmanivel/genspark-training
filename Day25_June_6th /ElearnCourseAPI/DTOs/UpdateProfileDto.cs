namespace ElearnAPI.DTOs
{
    public class UpdateProfileDto
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
    }
}
