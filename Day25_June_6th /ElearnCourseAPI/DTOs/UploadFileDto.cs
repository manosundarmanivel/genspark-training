using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ElearnAPI.DTOs
{
    public class UploadFileDto
    {
        [Required(ErrorMessage = "File is required.")]
       
        public IFormFile File { get; set; } = null!;

        [Required(ErrorMessage = "Course ID is required.")]
       
         public Guid CourseId { get; set; }
    }
}
