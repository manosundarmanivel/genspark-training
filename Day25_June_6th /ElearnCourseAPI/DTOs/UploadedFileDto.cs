using System;

namespace ElearnAPI.DTOs
{
    public class UploadedFileDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
        public string? FileType { get; set; }
        public string Path { get; set; } = null!;
        public Guid CourseId { get; set; }
        public string Topic { get; set; } = null!;
        public string? Description { get; set; }

        public bool? IsCompleted { get; set; } // from UserFileProgress (optional)
        public DateTime? CompletedAt { get; set; }
    }
}
