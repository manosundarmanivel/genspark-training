using System;
using System.Collections.Generic;

namespace ElearnAPI.Models
{
    public class Course
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public Guid InstructorId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

       
        public ICollection<UploadedFile> UploadedFiles { get; set; } = new List<UploadedFile>();
    }
}
