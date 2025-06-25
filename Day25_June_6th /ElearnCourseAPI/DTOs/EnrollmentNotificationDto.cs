// DTOs/EnrollmentNotificationDto.cs
namespace ElearnAPI.DTOs
{
    public class EnrollmentNotificationDto
    {
        public string StudentName { get; set; } = null!;
        public string CourseTitle { get; set; } = null!;
        public Guid InstructorId { get; set; }
    }
}
