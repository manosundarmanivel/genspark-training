using System;
using System.ComponentModel.DataAnnotations;

namespace ElearnAPI.DTOs
{
    public class CourseDto
    {
        
        public Guid Id { get; set; } 
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public Guid InstructorId { get; set; }
    }
}
