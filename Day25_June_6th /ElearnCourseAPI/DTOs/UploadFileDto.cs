using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace ElearnAPI.DTOs
{
    public class UploadFileDto
    {
      
        public IFormFile? File { get; set; }

        [Required(ErrorMessage = "Course ID is required.")]
        public Guid CourseId { get; set; }

        [Required(ErrorMessage = "Topic is required.")]
        public string Topic { get; set; } = null!;

        public string? Description { get; set; }
    }
}
