using System;
using System.Collections.Generic;

namespace ElearnAPI.DTOs
{
    public class CreateCourseDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public IFormFile? Thumbnail { get; set; } // this is for receiving the file
        public string Domain { get; set; } = null!;
        public string? Level { get; set; }
        public string? Language { get; set; }
        public List<string> Tags { get; set; } = new();
    }
}
