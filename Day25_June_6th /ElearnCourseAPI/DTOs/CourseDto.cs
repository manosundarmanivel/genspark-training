using System;
using System.Collections.Generic;

namespace ElearnAPI.DTOs
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public Guid InstructorId { get; set; }

        public List<UploadedFileDto> UploadedFiles { get; set; } = new();
    }
}
