using System;

namespace ElearnAPI.Models
{
    public class Enrollment
    {
        public Guid UserId { get; set; }
        public User Student { get; set; } = null!;

        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    }
}
