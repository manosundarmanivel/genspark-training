using System;
using System.Collections.Generic;

namespace ElearnAPI.Models
{
    public class Course
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? ThumbnailUrl { get; set; }


        public string Domain { get; set; } = null!; 

        public string? Level { get; set; } 

        public string? Language { get; set; } 
        public List<string> Tags { get; set; } = new();

        public decimal Price { get; set; } = 0.0m;

        public Guid InstructorId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public User Instructor { get; set; } = null!;

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public ICollection<UploadedFile> UploadedFiles { get; set; } = new List<UploadedFile>();
    }
}
