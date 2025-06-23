namespace ElearnAPI.DTOs
{
    public class UserDtoResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string? FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
