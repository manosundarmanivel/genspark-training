using System;
using System.Collections.Generic;

namespace ElearnAPI.DTOs
{
    public class StudentDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
