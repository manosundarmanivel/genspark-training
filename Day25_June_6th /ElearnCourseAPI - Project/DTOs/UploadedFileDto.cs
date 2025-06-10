namespace ElearnAPI.DTOs
{
    public class UploadedFileDto
    {
        public string FileName { get; set; } = null!;
        public string Path { get; set; } = null!;
        public string UploadedBy { get; set; } = null!;
        public DateTime UploadedAt { get; set; }
    }
}
