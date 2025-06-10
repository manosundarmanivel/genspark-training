using Microsoft.AspNetCore.Http;

namespace ElearnAPI.DTOs
{
    public class UploadFileDto
    {
        public IFormFile File { get; set; } = null!;
        public string CourseId { get; set; } = null!;
    }
}
