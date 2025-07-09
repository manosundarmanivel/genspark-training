namespace ElearnAPI.DTOs
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
       
    }
}
