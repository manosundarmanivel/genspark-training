using System;
using System.Collections.Generic;

namespace ElearnAPI.DTOs
{
public class CourseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string Domain { get; set; } = null!;
    public bool IsDeleted { get; set; }
    public string? Level { get; set; }
    public string? Language { get; set; }
    public List<string> Tags { get; set; } = new();
    public Guid InstructorId { get; set; }
    public List<UploadedFileDto> UploadedFiles { get; set; } = new();

    // 👇 Add this
    public List<UserDtoResponse> EnrolledStudents { get; set; } = new();
}


}
