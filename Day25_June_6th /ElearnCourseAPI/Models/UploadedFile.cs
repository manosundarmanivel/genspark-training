using System;

namespace ElearnAPI.Models
{
    public class UploadedFile
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string FileName { get; set; } = null!;

        public string? FileType { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public Guid CourseId { get; set; }


        public string Path { get; set; } = null!; 
        
         public string Topic { get; set; } = null!;

        
        public string? Description { get; set; }
    }
}
